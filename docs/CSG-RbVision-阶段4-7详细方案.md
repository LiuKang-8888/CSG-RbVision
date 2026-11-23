# CSG-RbVision - 阶段4至7详细方案

> 接续前文，本文档包含阶段4-7的详细实施计划

---

## 🎯 阶段4：业务逻辑层（2周）

### 目标
实现任务管理、工作流引擎、配方管理等核心业务逻辑

### 4.1 任务状态机（3天）

📝 **CSG.RbVision.Business/StateMachine/VisionTaskStateMachine.cs**

```csharp
// ⚠️ 告诉Cursor：使用Stateless库实现状态机
// NuGet: Stateless 5.15.0

public class VisionTaskStateMachine
{
    private readonly StateMachine<State, Trigger> _stateMachine;
    private readonly ILogger _logger;
    
    public enum State
    {
        Idle,              // 空闲
        WaitingTrigger,    // 等待触发
        Capturing,         // 采集中
        Processing,        // 处理中
        Analyzing,         // 分析中
        Outputting,        // 输出中
        Error,             // 错误
        Paused             // 暂停
    }
    
    public enum Trigger
    {
        Start,
        TriggerReceived,
        CaptureComplete,
        ProcessComplete,
        AnalysisComplete,
        OutputComplete,
        ErrorOccurred,
        Pause,
        Resume,
        Reset
    }
    
    public State CurrentState => _stateMachine.State;
    
    public event EventHandler<StateTransitionEventArgs>? StateChanged;
    
    public VisionTaskStateMachine(ILogger logger)
    {
        _logger = logger;
        _stateMachine = new StateMachine<State, Trigger>(State.Idle);
        
        ConfigureStateMachine();
    }
    
    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(State.Idle)
            .Permit(Trigger.Start, State.WaitingTrigger)
            .OnEntry(() => _logger.LogInformation("System idle"));
        
        _stateMachine.Configure(State.WaitingTrigger)
            .Permit(Trigger.TriggerReceived, State.Capturing)
            .Permit(Trigger.Pause, State.Paused)
            .Permit(Trigger.Reset, State.Idle)
            .OnEntry(() => _logger.LogInformation("Waiting for trigger"));
        
        _stateMachine.Configure(State.Capturing)
            .PermitReentry(Trigger.TriggerReceived) // 允许重新触发
            .Permit(Trigger.CaptureComplete, State.Processing)
            .Permit(Trigger.ErrorOccurred, State.Error)
            .OnEntryAsync(async () => await OnCapturingAsync());
        
        _stateMachine.Configure(State.Processing)
            .Permit(Trigger.ProcessComplete, State.Analyzing)
            .Permit(Trigger.ErrorOccurred, State.Error)
            .OnEntryAsync(async () => await OnProcessingAsync());
        
        _stateMachine.Configure(State.Analyzing)
            .Permit(Trigger.AnalysisComplete, State.Outputting)
            .Permit(Trigger.ErrorOccurred, State.Error)
            .OnEntryAsync(async () => await OnAnalyzingAsync());
        
        _stateMachine.Configure(State.Outputting)
            .Permit(Trigger.OutputComplete, State.WaitingTrigger)
            .Permit(Trigger.ErrorOccurred, State.Error)
            .OnEntryAsync(async () => await OnOutputtingAsync());
        
        _stateMachine.Configure(State.Error)
            .Permit(Trigger.Reset, State.Idle)
            .OnEntry(() => _logger.LogError("System error state"));
        
        _stateMachine.Configure(State.Paused)
            .Permit(Trigger.Resume, State.WaitingTrigger)
            .Permit(Trigger.Reset, State.Idle)
            .OnEntry(() => _logger.LogWarning("System paused"));
        
        // 状态变化通知
        _stateMachine.OnTransitioned(t =>
        {
            _logger.LogInformation($"State: {t.Source} -> {t.Destination} (Trigger: {t.Trigger})");
            StateChanged?.Invoke(this, new StateTransitionEventArgs(t.Source, t.Destination));
        });
    }
    
    public async Task FireAsync(Trigger trigger)
    {
        if (_stateMachine.CanFire(trigger))
        {
            await _stateMachine.FireAsync(trigger);
        }
        else
        {
            _logger.LogWarning($"Cannot fire trigger {trigger} in state {CurrentState}");
        }
    }
    
    private async Task OnCapturingAsync()
    {
        // 由外部注入的采集逻辑
        await Task.CompletedTask;
    }
    
    private async Task OnProcessingAsync()
    {
        await Task.CompletedTask;
    }
    
    private async Task OnAnalyzingAsync()
    {
        await Task.CompletedTask;
    }
    
    private async Task OnOutputtingAsync()
    {
        await Task.CompletedTask;
    }
}
```

### 4.2 工作流引擎（4天）

📝 **CSG.RbVision.Business/Workflow/WorkflowEngine.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 工作流支持串行、并行、条件分支
// 2. 每个步骤可以是算法或自定义操作
// 3. 支持流程暂停、恢复

public class WorkflowEngine
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    
    public WorkflowEngine(ILogger<WorkflowEngine> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<WorkflowResult> ExecuteAsync(Workflow workflow, ImageData input, 
        CancellationToken cancellationToken = default)
    {
        var context = new WorkflowContext
        {
            InputImage = input,
            Variables = new Dictionary<string, object>()
        };
        
        var stopwatch = Stopwatch.StartNew();
        var stepResults = new List<StepResult>();
        
        try
        {
            foreach (var step in workflow.Steps)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return WorkflowResult.Cancelled(stepResults);
                }
                
                var stepResult = await ExecuteStepAsync(step, context, cancellationToken);
                stepResults.Add(stepResult);
                
                // 更新上下文
                context.Variables[step.Name] = stepResult.Output;
                
                // 检查步骤是否成功
                if (!stepResult.Success && step.IsRequired)
                {
                    _logger.LogWarning($"Required step '{step.Name}' failed, stopping workflow");
                    return WorkflowResult.Failed(stepResults, step.Name);
                }
                
                // 条件分支
                if (step.HasCondition && !EvaluateCondition(step.Condition!, context))
                {
                    _logger.LogInformation($"Step '{step.Name}' condition not met, skipping");
                    continue;
                }
            }
            
            stopwatch.Stop();
            
            return WorkflowResult.Success(stepResults, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Workflow execution failed");
            return WorkflowResult.Error(stepResults, ex.Message);
        }
    }
    
    private async Task<StepResult> ExecuteStepAsync(WorkflowStep step, 
        WorkflowContext context, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation($"Executing step: {step.Name}");
            
            object? output = step.StepType switch
            {
                StepType.Algorithm => await ExecuteAlgorithmStepAsync(step, context, cancellationToken),
                StepType.ImageProcessing => await ExecuteImageProcessingStepAsync(step, context),
                StepType.DataOperation => await ExecuteDataOperationStepAsync(step, context),
                StepType.CustomAction => await ExecuteCustomActionStepAsync(step, context),
                _ => throw new NotSupportedException($"Step type {step.StepType} not supported")
            };
            
            sw.Stop();
            
            return StepResult.Success(step.Name, output, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Step '{step.Name}' failed");
            return StepResult.Failed(step.Name, ex.Message);
        }
    }
    
    private async Task<object?> ExecuteAlgorithmStepAsync(WorkflowStep step, 
        WorkflowContext context, CancellationToken cancellationToken)
    {
        // 通过依赖注入获取算法实例
        var algorithmType = Type.GetType(step.AlgorithmTypeName!);
        if (algorithmType == null)
            throw new InvalidOperationException($"Algorithm type not found: {step.AlgorithmTypeName}");
        
        var algorithm = _serviceProvider.GetService(algorithmType) as IVisionAlgorithm;
        if (algorithm == null)
            throw new InvalidOperationException($"Failed to resolve algorithm: {algorithmType.Name}");
        
        // 解析参数（支持从上下文变量引用）
        var parameters = ResolveParameters(step.Parameters, context);
        
        return await algorithm.ExecuteAsync(context.InputImage, parameters);
    }
    
    private Dictionary<string, object> ResolveParameters(
        Dictionary<string, object> parameters, WorkflowContext context)
    {
        var resolved = new Dictionary<string, object>();
        
        foreach (var kvp in parameters)
        {
            // 支持变量引用：${variableName}
            if (kvp.Value is string strValue && strValue.StartsWith("${") && strValue.EndsWith("}"))
            {
                var varName = strValue[2..^1];
                resolved[kvp.Key] = context.Variables.ContainsKey(varName) 
                    ? context.Variables[varName] 
                    : kvp.Value;
            }
            else
            {
                resolved[kvp.Key] = kvp.Value;
            }
        }
        
        return resolved;
    }
    
    private bool EvaluateCondition(string condition, WorkflowContext context)
    {
        // 简单的条件评估（可扩展为表达式解析器）
        // 示例：${previousStep.Success} == true
        // 这里使用简化实现
        return true; // TODO: 实现完整的条件表达式解析
    }
}

public class Workflow
{
    public string Name { get; set; } = string.Empty;
    public List<WorkflowStep> Steps { get; set; } = new();
}

public class WorkflowStep
{
    public string Name { get; set; } = string.Empty;
    public StepType StepType { get; set; }
    public string? AlgorithmTypeName { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsRequired { get; set; } = true;
    public bool HasCondition { get; set; }
    public string? Condition { get; set; }
}

public enum StepType
{
    Algorithm,
    ImageProcessing,
    DataOperation,
    CustomAction
}

public class WorkflowContext
{
    public ImageData InputImage { get; set; } = null!;
    public Dictionary<string, object> Variables { get; set; } = new();
}
```

### 4.3 配方管理器（3天）

📝 **CSG.RbVision.Business/Recipe/RecipeManager.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 配方包含相机参数、光源配置、算法流程、判定标准
// 2. 支持配方版本管理和快速切换
// 3. 切换前需要验证配方有效性

public class RecipeManager
{
    private readonly ILogger _logger;
    private readonly IRecipeRepository _repository;
    private readonly ICameraManager _cameraManager;
    private readonly ILightController _lightController;
    private readonly WorkflowEngine _workflowEngine;
    
    private Recipe? _currentRecipe;
    private readonly SemaphoreSlim _switchLock = new(1, 1);
    
    public Recipe? CurrentRecipe => _currentRecipe;
    
    public event EventHandler<RecipeSwitchedEventArgs>? RecipeSwitched;
    
    public RecipeManager(
        ILogger<RecipeManager> logger,
        IRecipeRepository repository,
        ICameraManager cameraManager,
        ILightController lightController,
        WorkflowEngine workflowEngine)
    {
        _logger = logger;
        _repository = repository;
        _cameraManager = cameraManager;
        _lightController = lightController;
        _workflowEngine = workflowEngine;
    }
    
    public async Task<Result<Recipe>> LoadRecipeAsync(string recipeName)
    {
        try
        {
            var recipe = await _repository.GetByNameAsync(recipeName);
            if (recipe == null)
                return Result<Recipe>.Failure(ErrorCode.RecipeNotFound, 
                    $"Recipe '{recipeName}' not found");
            
            return Result<Recipe>.Success(recipe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to load recipe: {recipeName}");
            return Result<Recipe>.Failure(ErrorCode.RecipeLoadFailed, ex.Message);
        }
    }
    
    public async Task<Result<bool>> SwitchRecipeAsync(string recipeName, 
        bool validateBeforeSwitch = true)
    {
        await _switchLock.WaitAsync();
        
        try
        {
            _logger.LogInformation($"Switching to recipe: {recipeName}");
            
            // 1. 加载配方
            var loadResult = await LoadRecipeAsync(recipeName);
            if (!loadResult.IsSuccess)
                return Result<bool>.Failure(loadResult.ErrorCode, loadResult.ErrorMessage!);
            
            var newRecipe = loadResult.Value!;
            
            // 2. 验证配方
            if (validateBeforeSwitch)
            {
                var validationResult = await ValidateRecipeAsync(newRecipe);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning($"Recipe validation failed: {validationResult.ErrorMessage}");
                    return validationResult;
                }
            }
            
            // 3. 保存当前状态（用于回滚）
            var previousRecipe = _currentRecipe;
            
            try
            {
                // 4. 应用相机配置
                await ApplyCameraConfigurationAsync(newRecipe.CameraConfig);
                
                // 5. 应用光源配置
                await ApplyLightConfigurationAsync(newRecipe.LightConfig);
                
                // 6. 构建工作流
                var workflow = BuildWorkflowFromRecipe(newRecipe);
                
                // 7. 更新当前配方
                _currentRecipe = newRecipe;
                
                _logger.LogInformation($"Recipe switched successfully: {recipeName}");
                
                RecipeSwitched?.Invoke(this, new RecipeSwitchedEventArgs(previousRecipe, newRecipe));
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply recipe, rolling back");
                
                // 回滚
                if (previousRecipe != null)
                {
                    await SwitchRecipeAsync(previousRecipe.Name, false);
                }
                
                return Result<bool>.Failure(ErrorCode.RecipeSwitchFailed, ex.Message);
            }
        }
        finally
        {
            _switchLock.Release();
        }
    }
    
    private async Task<Result<bool>> ValidateRecipeAsync(Recipe recipe)
    {
        // 验证相机参数范围
        if (recipe.CameraConfig.ExposureTime < 100 || recipe.CameraConfig.ExposureTime > 1000000)
            return Result<bool>.Failure(ErrorCode.InvalidRecipe, "Invalid exposure time");
        
        // 验证算法步骤完整性
        if (recipe.AlgorithmSteps.Count == 0)
            return Result<bool>.Failure(ErrorCode.InvalidRecipe, "No algorithm steps defined");
        
        // 验证模板文件存在性
        foreach (var step in recipe.AlgorithmSteps)
        {
            if (step.Parameters.ContainsKey("TemplatePath"))
            {
                var templatePath = step.Parameters["TemplatePath"] as string;
                if (!File.Exists(templatePath))
                    return Result<bool>.Failure(ErrorCode.InvalidRecipe, 
                        $"Template not found: {templatePath}");
            }
        }
        
        return Result<bool>.Success(true);
    }
    
    private async Task ApplyCameraConfigurationAsync(CameraConfiguration config)
    {
        await _cameraManager.SetExposureTimeAsync(config.ExposureTime);
        await _cameraManager.SetGainAsync(config.Gain);
        await _cameraManager.SetTriggerModeAsync(config.TriggerMode);
        
        if (config.ROI != null)
        {
            await _cameraManager.SetROIAsync(config.ROI.Value);
        }
    }
    
    private async Task ApplyLightConfigurationAsync(LightConfiguration config)
    {
        for (int i = 0; i < config.Channels.Count; i++)
        {
            await _lightController.SetBrightnessAsync(i, config.Channels[i].Brightness);
        }
    }
    
    private Workflow BuildWorkflowFromRecipe(Recipe recipe)
    {
        var workflow = new Workflow
        {
            Name = recipe.Name,
            Steps = recipe.AlgorithmSteps.Select(step => new WorkflowStep
            {
                Name = step.Name,
                StepType = StepType.Algorithm,
                AlgorithmTypeName = step.AlgorithmType,
                Parameters = step.Parameters,
                IsRequired = step.IsRequired
            }).ToList()
        };
        
        return workflow;
    }
    
    public async Task<Result<bool>> SaveRecipeAsync(Recipe recipe)
    {
        try
        {
            await _repository.SaveAsync(recipe);
            _logger.LogInformation($"Recipe saved: {recipe.Name}");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to save recipe: {recipe.Name}");
            return Result<bool>.Failure(ErrorCode.RecipeSaveFailed, ex.Message);
        }
    }
    
    public async Task<List<RecipeInfo>> GetAllRecipesAsync()
    {
        return await _repository.GetAllRecipeInfoAsync();
    }
}

// 配方数据模型
public class Recipe
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ProductModel { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public DateTime ModifiedTime { get; set; } = DateTime.Now;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public CameraConfiguration CameraConfig { get; set; } = new();
    public LightConfiguration LightConfig { get; set; } = new();
    public List<AlgorithmStepConfig> AlgorithmSteps { get; set; } = new();
    public QualityStandards QualityStandards { get; set; } = new();
    public IOMapping IOMapping { get; set; } = new();
    
    public Dictionary<string, object> ExtendedProperties { get; set; } = new();
}

public class CameraConfiguration
{
    public double ExposureTime { get; set; } = 10000; // 微秒
    public double Gain { get; set; } = 1.0;
    public TriggerMode TriggerMode { get; set; } = TriggerMode.Software;
    public Rect? ROI { get; set; }
}

public class LightConfiguration
{
    public List<LightChannel> Channels { get; set; } = new();
}

public class LightChannel
{
    public int Brightness { get; set; } = 128; // 0-255
    public bool Enabled { get; set; } = true;
}

public class AlgorithmStepConfig
{
    public string Name { get; set; } = string.Empty;
    public string AlgorithmType { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public bool IsRequired { get; set; } = true;
}

public class QualityStandards
{
    public double MinScore { get; set; } = 0.8;
    public int MaxDefectCount { get; set; } = 0;
    public Dictionary<string, (double Min, double Max)> MeasurementRanges { get; set; } = new();
}

public class IOMapping
{
    public int TriggerInputPin { get; set; } = 0;
    public int OKOutputPin { get; set; } = 1;
    public int NGOutputPin { get; set; } = 2;
}
```

### 4.4 任务执行器（2天）

📝 **CSG.RbVision.Business/Execution/VisionTaskExecutor.cs**

```csharp
public class VisionTaskExecutor
{
    private readonly ILogger _logger;
    private readonly ICameraManager _cameraManager;
    private readonly WorkflowEngine _workflowEngine;
    private readonly RecipeManager _recipeManager;
    private readonly IResultProcessor _resultProcessor;
    private readonly VisionTaskStateMachine _stateMachine;
    
    private readonly Channel<TriggerSignal> _triggerChannel;
    private CancellationTokenSource? _executionCts;
    
    public VisionTaskExecutor(
        ILogger<VisionTaskExecutor> logger,
        ICameraManager cameraManager,
        WorkflowEngine workflowEngine,
        RecipeManager recipeManager,
        IResultProcessor resultProcessor,
        VisionTaskStateMachine stateMachine)
    {
        _logger = logger;
        _cameraManager = cameraManager;
        _workflowEngine = workflowEngine;
        _recipeManager = recipeManager;
        _resultProcessor = resultProcessor;
        _stateMachine = stateMachine;
        
        _triggerChannel = Channel.CreateBounded<TriggerSignal>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });
    }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _executionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.Start);
        
        // 启动执行循环
        _ = Task.Run(() => ExecutionLoopAsync(_executionCts.Token), _executionCts.Token);
        
        _logger.LogInformation("Vision task executor started");
    }
    
    public async Task StopAsync()
    {
        _executionCts?.Cancel();
        await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.Reset);
        _logger.LogInformation("Vision task executor stopped");
    }
    
    public void Trigger(TriggerSource source = TriggerSource.Software)
    {
        _triggerChannel.Writer.TryWrite(new TriggerSignal
        {
            Source = source,
            Timestamp = DateTime.Now
        });
    }
    
    private async Task ExecutionLoopAsync(CancellationToken cancellationToken)
    {
        await foreach (var trigger in _triggerChannel.Reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.TriggerReceived);
                
                var result = await ExecuteSingleCycleAsync(trigger, cancellationToken);
                
                await _resultProcessor.ProcessResultAsync(result);
                
                await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.OutputComplete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Execution cycle failed");
                await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.ErrorOccurred);
            }
        }
    }
    
    private async Task<VisionResult> ExecuteSingleCycleAsync(
        TriggerSignal trigger, CancellationToken cancellationToken)
    {
        var cycleStopwatch = Stopwatch.StartNew();
        var result = new VisionResult
        {
            Id = Guid.NewGuid(),
            Timestamp = trigger.Timestamp,
            RecipeName = _recipeManager.CurrentRecipe?.Name ?? "Unknown"
        };
        
        try
        {
            // 1. 采集图像
            await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.CaptureComplete);
            var captureResult = await _cameraManager.GrabImageAsync(cancellationToken);
            
            if (!captureResult.IsSuccess)
            {
                result.Success = false;
                result.ErrorMessage = captureResult.ErrorMessage;
                return result;
            }
            
            result.Image = captureResult.Value!;
            result.CaptureTimeMs = cycleStopwatch.ElapsedMilliseconds;
            
            // 2. 执行工作流
            await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.ProcessComplete);
            
            var recipe = _recipeManager.CurrentRecipe;
            if (recipe == null)
            {
                result.Success = false;
                result.ErrorMessage = "No recipe loaded";
                return result;
            }
            
            var workflow = BuildWorkflowFromRecipe(recipe);
            var workflowResult = await _workflowEngine.ExecuteAsync(workflow, result.Image, cancellationToken);
            
            result.WorkflowResult = workflowResult;
            result.ProcessingTimeMs = workflowResult.TotalTimeMs;
            
            // 3. 判定
            await _stateMachine.FireAsync(VisionTaskStateMachine.Trigger.AnalysisComplete);
            result.Decision = EvaluateDecision(workflowResult, recipe.QualityStandards);
            result.Success = true;
            
            cycleStopwatch.Stop();
            result.TotalCycleTimeMs = cycleStopwatch.ElapsedMilliseconds;
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Single cycle execution failed");
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }
    
    private Decision EvaluateDecision(WorkflowResult workflowResult, QualityStandards standards)
    {
        if (!workflowResult.Success)
            return Decision.NG;
        
        // 检查所有步骤结果
        foreach (var stepResult in workflowResult.StepResults)
        {
            if (!stepResult.Success)
                return Decision.NG;
            
            // 检查分数
            if (stepResult.Output is AlgorithmResult algoResult)
            {
                if (algoResult is MatchingResult matchResult && 
                    matchResult.Score < standards.MinScore)
                {
                    return Decision.NG;
                }
            }
        }
        
        return Decision.OK;
    }
}

public class TriggerSignal
{
    public TriggerSource Source { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum TriggerSource
{
    Software,
    Hardware,
    Timer
}

public class VisionResult
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    
    public ImageData? Image { get; set; }
    public WorkflowResult? WorkflowResult { get; set; }
    public Decision Decision { get; set; }
    
    public long CaptureTimeMs { get; set; }
    public long ProcessingTimeMs { get; set; }
    public long TotalCycleTimeMs { get; set; }
}

public enum Decision
{
    Unknown,
    OK,
    NG
}
```

### 阶段4测试标准

**单元测试**：
```csharp
public class VisionTaskStateMachineTests
{
    [Fact]
    public async Task StateMachine_ShouldTransitionCorrectly()
    {
        // Arrange
        var logger = Mock.Of<ILogger<VisionTaskStateMachine>>();
        var sm = new VisionTaskStateMachine(logger);
        
        // Act & Assert
        Assert.Equal(VisionTaskStateMachine.State.Idle, sm.CurrentState);
        
        await sm.FireAsync(VisionTaskStateMachine.Trigger.Start);
        Assert.Equal(VisionTaskStateMachine.State.WaitingTrigger, sm.CurrentState);
        
        await sm.FireAsync(VisionTaskStateMachine.Trigger.TriggerReceived);
        Assert.Equal(VisionTaskStateMachine.State.Capturing, sm.CurrentState);
    }
    
    [Fact]
    public async Task StateMachine_ShouldNotAllowInvalidTransition()
    {
        var logger = Mock.Of<ILogger<VisionTaskStateMachine>>();
        var sm = new VisionTaskStateMachine(logger);
        
        // 在Idle状态不能直接触发
        await sm.FireAsync(VisionTaskStateMachine.Trigger.TriggerReceived);
        
        // 状态应该不变
        Assert.Equal(VisionTaskStateMachine.State.Idle, sm.CurrentState);
    }
}

public class RecipeManagerTests
{
    [Fact]
    public async Task SwitchRecipe_ShouldApplyConfiguration()
    {
        // 测试配方切换
    }
    
    [Fact]
    public async Task SwitchRecipe_ShouldRollbackOnFailure()
    {
        // 测试失败回滚
    }
}

public class WorkflowEngineTests
{
    [Fact]
    public async Task Workflow_ShouldExecuteStepsInOrder()
    {
        // 测试工作流顺序执行
    }
    
    [Fact]
    public async Task Workflow_ShouldStopOnRequiredStepFailure()
    {
        // 测试必选步骤失败时停止
    }
}
```

**集成测试**：
- [ ] 完整执行周期：触发→采集→处理→判定→输出
- [ ] 配方切换：参数正确应用到硬件
- [ ] 状态机：所有状态转换路径正确
- [ ] 工作流：串行、并行、条件分支正确执行

**性能测试**：
- [ ] 完整周期时间<500ms
- [ ] 配方切换时间<2s
- [ ] 1000次循环无内存泄漏
- [ ] 并发10个工作流无阻塞

---

## 🎯 阶段5：WPF用户界面（3周）

### 目标
实现操作友好的WPF界面，包括实时显示、参数配置、统计分析

### 5.1 MVVM基础架构（2天）

📝 **CSG.RbVision.UI.WPF/ViewModels/ViewModelBase.cs**

```csharp
// ⚠️ 告诉Cursor：使用CommunityToolkit.Mvvm简化MVVM开发
// NuGet: CommunityToolkit.Mvvm 8.2.2

public abstract class ViewModelBase : ObservableObject
{
    protected readonly ILogger _logger;
    protected readonly IServiceProvider _serviceProvider;
    
    protected ViewModelBase(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected async Task ExecuteWithExceptionHandlingAsync(Func<Task> action, 
        string errorMessage = "Operation failed")
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, errorMessage);
            await ShowErrorAsync($"{errorMessage}: {ex.Message}");
        }
    }
    
    protected virtual Task ShowErrorAsync(string message)
    {
        // 由子类实现或通过消息服务显示
        return Task.CompletedTask;
    }
}
```

### 5.2 主界面ViewModel（3天）

📝 **CSG.RbVision.UI.WPF/ViewModels/MainViewModel.cs**

```csharp
public partial class MainViewModel : ViewModelBase
{
    private readonly VisionTaskExecutor _taskExecutor;
    private readonly RecipeManager _recipeManager;
    private readonly IResultRepository _resultRepository;
    
    [ObservableProperty]
    private ImageSource? _currentImage;
    
    [ObservableProperty]
    private string _currentRecipeName = "No Recipe";
    
    [ObservableProperty]
    private string _systemState = "Idle";
    
    [ObservableProperty]
    private int _totalCount;
    
    [ObservableProperty]
    private int _okCount;
    
    [ObservableProperty]
    private int _ngCount;
    
    [ObservableProperty]
    private double _yieldRate;
    
    [ObservableProperty]
    private double _cycleTime;
    
    [ObservableProperty]
    private bool _isRunning;
    
    public ObservableCollection<ResultViewModel> RecentResults { get; } = new();
    
    public MainViewModel(
        ILogger<MainViewModel> logger,
        IServiceProvider serviceProvider,
        VisionTaskExecutor taskExecutor,
        RecipeManager recipeManager,
        IResultRepository resultRepository) 
        : base(logger, serviceProvider)
    {
        _taskExecutor = taskExecutor;
        _recipeManager = recipeManager;
        _resultRepository = resultRepository;
        
        // 订阅事件
        _taskExecutor.ResultProcessed += OnResultProcessed;
        _recipeManager.RecipeSwitched += OnRecipeSwitched;
    }
    
    [RelayCommand]
    private async Task StartAsync()
    {
        await ExecuteWithExceptionHandlingAsync(async () =>
        {
            await _taskExecutor.StartAsync();
            IsRunning = true;
            SystemState = "Running";
        }, "Failed to start");
    }
    
    [RelayCommand]
    private async Task StopAsync()
    {
        await ExecuteWithExceptionHandlingAsync(async () =>
        {
            await _taskExecutor.StopAsync();
            IsRunning = false;
            SystemState = "Stopped";
        }, "Failed to stop");
    }
    
    [RelayCommand]
    private void Trigger()
    {
        if (IsRunning)
        {
            _taskExecutor.Trigger();
        }
    }
    
    [RelayCommand]
    private async Task LoadRecipeAsync(string recipeName)
    {
        await ExecuteWithExceptionHandlingAsync(async () =>
        {
            var result = await _recipeManager.SwitchRecipeAsync(recipeName);
            if (result.IsSuccess)
            {
                CurrentRecipeName = recipeName;
            }
        }, $"Failed to load recipe: {recipeName}");
    }
    
    private void OnResultProcessed(object? sender, VisionResult result)
    {
        // ⚠️ 在UI线程更新
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            // 更新统计
            TotalCount++;
            if (result.Decision == Decision.OK)
                OkCount++;
            else
                NgCount++;
            
            YieldRate = TotalCount > 0 ? (double)OkCount / TotalCount * 100 : 0;
            CycleTime = result.TotalCycleTimeMs;
            
            // 更新图像显示
            if (result.Image != null)
            {
                CurrentImage = ConvertToImageSource(result.Image);
            }
            
            // 添加到结果列表（保留最近100条）
            RecentResults.Insert(0, new ResultViewModel(result));
            if (RecentResults.Count > 100)
            {
                RecentResults.RemoveAt(RecentResults.Count - 1);
            }
        });
    }
    
    private ImageSource? ConvertToImageSource(ImageData imageData)
    {
        try
        {
            using var mat = imageData.ToMat();
            var bitmap = new WriteableBitmap(mat.Width, mat.Height, 96, 96, 
                PixelFormats.Bgr24, null);
            
            bitmap.Lock();
            try
            {
                unsafe
                {
                    Buffer.MemoryCopy(
                        mat.DataPointer.ToPointer(),
                        bitmap.BackBuffer.ToPointer(),
                        bitmap.BackBufferStride * bitmap.PixelHeight,
                        mat.Step() * mat.Height);
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, mat.Width, mat.Height));
            }
            finally
            {
                bitmap.Unlock();
            }
            
            bitmap.Freeze(); // 允许跨线程访问
            return bitmap;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert image");
            return null;
        }
    }
}
```

### 5.3 高性能图像显示控件（2天）

📝 **CSG.RbVision.UI.WPF/Controls/FastImageViewer.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 使用WriteableBitmap提高性能
// 2. 支持缩放、平移、ROI绘制
// 3. 避免在UI线程做图像转换

public class FastImageViewer : FrameworkElement
{
    private WriteableBitmap? _displayBitmap;
    private readonly Channel<ImageData> _imageQueue;
    private Task? _renderTask;
    private CancellationTokenSource? _cts;
    
    private double _scale = 1.0;
    private Point _offset = new(0, 0);
    private Point _lastMousePos;
    private bool _isPanning = false;
    
    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageData),
            typeof(FastImageViewer), new PropertyMetadata(null, OnImageSourceChanged));
    
    public ImageData? ImageSource
    {
        get => (ImageData?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
    
    public FastImageViewer()
    {
        _imageQueue = Channel.CreateBounded<ImageData>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });
        
        ClipToBounds = true;
        
        MouseWheel += OnMouseWheel;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
        MouseMove += OnMouseMove;
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _cts = new CancellationTokenSource();
        _renderTask = Task.Run(() => RenderLoopAsync(_cts.Token));
    }
    
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
    }
    
    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var viewer = (FastImageViewer)d;
        var image = (ImageData?)e.NewValue;
        
        if (image != null)
        {
            viewer._imageQueue.Writer.TryWrite(image);
        }
    }
    
    private async Task RenderLoopAsync(CancellationToken cancellationToken)
    {
        await foreach (var image in _imageQueue.Reader.ReadAllAsync(cancellationToken))
        {
            // 在后台线程转换图像
            var bitmap = ConvertToBitmap(image);
            
            // 切换到UI线程更新
            await Dispatcher.InvokeAsync(() =>
            {
                _displayBitmap = bitmap;
                InvalidateVisual();
            }, DispatcherPriority.Render, cancellationToken);
        }
    }
    
    private WriteableBitmap ConvertToBitmap(ImageData image)
    {
        using var mat = image.ToMat();
        
        var bitmap = new WriteableBitmap(mat.Width, mat.Height, 96, 96, 
            PixelFormats.Bgr24, null);
        
        bitmap.Lock();
        try
        {
            unsafe
            {
                Buffer.MemoryCopy(
                    mat.DataPointer.ToPointer(),
                    bitmap.BackBuffer.ToPointer(),
                    bitmap.BackBufferStride * bitmap.PixelHeight,
                    mat.Step() * mat.Height);
            }
            bitmap.AddDirtyRect(new Int32Rect(0, 0, mat.Width, mat.Height));
        }
        finally
        {
            bitmap.Unlock();
        }
        
        bitmap.Freeze();
        return bitmap;
    }
    
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        
        // 背景
        dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, ActualWidth, ActualHeight));
        
        if (_displayBitmap != null)
        {
            // 应用缩放和平移
            dc.PushTransform(new TranslateTransform(_offset.X, _offset.Y));
            dc.PushTransform(new ScaleTransform(_scale, _scale));
            
            dc.DrawImage(_displayBitmap, new Rect(0, 0, _displayBitmap.PixelWidth, _displayBitmap.PixelHeight));
            
            dc.Pop();
            dc.Pop();
        }
    }
    
    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // 缩放
        double scaleFactor = e.Delta > 0 ? 1.1 : 0.9;
        _scale *= scaleFactor;
        _scale = Math.Clamp(_scale, 0.1, 10.0);
        
        InvalidateVisual();
    }
    
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isPanning = true;
        _lastMousePos = e.GetPosition(this);
        CaptureMouse();
    }
    
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isPanning = false;
        ReleaseMouseCapture();
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isPanning)
        {
            var currentPos = e.GetPosition(this);
            _offset.X += currentPos.X - _lastMousePos.X;
            _offset.Y += currentPos.Y - _lastMousePos.Y;
            _lastMousePos = currentPos;
            
            InvalidateVisual();
        }
    }
}
```

### 5.4 配方编辑界面（3天）

📝 **CSG.RbVision.UI.WPF/ViewModels/RecipeEditorViewModel.cs**

```csharp
public partial class RecipeEditorViewModel : ViewModelBase
{
    private readonly RecipeManager _recipeManager;
    
    [ObservableProperty]
    private Recipe? _currentRecipe;
    
    [ObservableProperty]
    private bool _isModified;
    
    public ObservableCollection<AlgorithmStepViewModel> AlgorithmSteps { get; } = new();
    
    public RecipeEditorViewModel(
        ILogger<RecipeEditorViewModel> logger,
        IServiceProvider serviceProvider,
        RecipeManager recipeManager) 
        : base(logger, serviceProvider)
    {
        _recipeManager = recipeManager;
    }
    
    [RelayCommand]
    private async Task LoadRecipeAsync(string recipeName)
    {
        var result = await _recipeManager.LoadRecipeAsync(recipeName);
        if (result.IsSuccess)
        {
            CurrentRecipe = result.Value;
            LoadAlgorithmSteps(result.Value!);
            IsModified = false;
        }
    }
    
    [RelayCommand]
    private async Task SaveRecipeAsync()
    {
        if (CurrentRecipe == null)
            return;
        
        var result = await _recipeManager.SaveRecipeAsync(CurrentRecipe);
        if (result.IsSuccess)
        {
            IsModified = false;
        }
    }
    
    [RelayCommand]
    private void AddAlgorithmStep()
    {
        var step = new AlgorithmStepViewModel
        {
            Name = $"Step {AlgorithmSteps.Count + 1}",
            AlgorithmType = "TemplateMatching"
        };
        
        AlgorithmSteps.Add(step);
        IsModified = true;
    }
    
    [RelayCommand]
    private void RemoveAlgorithmStep(AlgorithmStepViewModel step)
    {
        AlgorithmSteps.Remove(step);
        IsModified = true;
    }
    
    [RelayCommand]
    private void MoveStepUp(AlgorithmStepViewModel step)
    {
        int index = AlgorithmSteps.IndexOf(step);
        if (index > 0)
        {
            AlgorithmSteps.Move(index, index - 1);
            IsModified = true;
        }
    }
    
    [RelayCommand]
    private void MoveStepDown(AlgorithmStepViewModel step)
    {
        int index = AlgorithmSteps.IndexOf(step);
        if (index < AlgorithmSteps.Count - 1)
        {
            AlgorithmSteps.Move(index, index + 1);
            IsModified = true;
        }
    }
    
    private void LoadAlgorithmSteps(Recipe recipe)
    {
        AlgorithmSteps.Clear();
        foreach (var step in recipe.AlgorithmSteps)
        {
            AlgorithmSteps.Add(new AlgorithmStepViewModel
            {
                Name = step.Name,
                AlgorithmType = step.AlgorithmType,
                Parameters = new ObservableCollection<ParameterViewModel>(
                    step.Parameters.Select(kvp => new ParameterViewModel
                    {
                        Key = kvp.Key,
                        Value = kvp.Value.ToString()
                    }))
            });
        }
    }
}

public partial class AlgorithmStepViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private string _algorithmType = string.Empty;
    
    [ObservableProperty]
    private bool _isRequired = true;
    
    public ObservableCollection<ParameterViewModel> Parameters { get; } = new();
}

public partial class ParameterViewModel : ObservableObject
{
    [ObservableProperty]
    private string _key = string.Empty;
    
    [ObservableProperty]
    private string? _value;
}
```

### 5.5 统计分析界面（2天）

📝 **CSG.RbVision.UI.WPF/ViewModels/StatisticsViewModel.cs**

```csharp
public partial class StatisticsViewModel : ViewModelBase
{
    private readonly IResultRepository _resultRepository;
    private readonly System.Timers.Timer _refreshTimer;
    
    [ObservableProperty]
    private ProductionStatistics? _currentShiftStats;
    
    [ObservableProperty]
    private ChartData? _yieldTrendData;
    
    [ObservableProperty]
    private ChartData? _cycleTimeData;
    
    public ObservableCollection<DefectStatistic> DefectDistribution { get; } = new();
    
    public StatisticsViewModel(
        ILogger<StatisticsViewModel> logger,
        IServiceProvider serviceProvider,
        IResultRepository resultRepository) 
        : base(logger, serviceProvider)
    {
        _resultRepository = resultRepository;
        
        _refreshTimer = new System.Timers.Timer(5000); // 5秒刷新
        _refreshTimer.Elapsed += async (s, e) => await RefreshStatisticsAsync();
        _refreshTimer.Start();
    }
    
    [RelayCommand]
    private async Task RefreshStatisticsAsync()
    {
        await ExecuteWithExceptionHandlingAsync(async () =>
        {
            var now = DateTime.Now;
            var shiftStart = now.Date.AddHours(8); // 假设8点开始
            if (now.Hour < 8)
                shiftStart = shiftStart.AddDays(-1);
            
            CurrentShiftStats = await _resultRepository.GetStatisticsAsync(shiftStart, now);
            
            // 更新图表数据
            await UpdateChartDataAsync();
            
            // 更新缺陷分布
            var defects = await _resultRepository.GetDefectDistributionAsync(shiftStart, now);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                DefectDistribution.Clear();
                foreach (var defect in defects)
                {
                    DefectDistribution.Add(defect);
                }
            });
        }, "Failed to refresh statistics");
    }
    
    private async Task UpdateChartDataAsync()
    {
        // 生成最近24小时的合格率趋势
        var now = DateTime.Now;
        var start = now.AddHours(-24);
        
        var hourlyStats = await _resultRepository.GetHourlyStatisticsAsync(start, now);
        
        YieldTrendData = new ChartData
        {
            Labels = hourlyStats.Select(s => s.Hour.ToString("HH:mm")).ToList(),
            Values = hourlyStats.Select(s => s.YieldRate).ToList()
        };
        
        CycleTimeData = new ChartData
        {
            Labels = hourlyStats.Select(s => s.Hour.ToString("HH:mm")).ToList(),
            Values = hourlyStats.Select(s => s.AverageCycleTime).ToList()
        };
    }
}

public class ProductionStatistics
{
    public int TotalCount { get; set; }
    public int OKCount { get; set; }
    public int NGCount { get; set; }
    public double YieldRate => TotalCount > 0 ? (double)OKCount / TotalCount * 100 : 0;
    public double AverageCycleTime { get; set; }
}

public class ChartData
{
    public List<string> Labels { get; set; } = new();
    public List<double> Values { get; set; } = new();
}

public class DefectStatistic
{
    public string DefectType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
```

### 阶段5测试标准

**UI测试**（手动）：
- [ ] 主界面响应流畅，无卡顿
- [ ] 图像显示帧率≥10fps
- [ ] 统计数据实时更新
- [ ] 配方编辑保存正确
- [ ] 窗口缩放正常显示

**自动化UI测试**（使用FlaUI）：
```csharp
[TestClass]
public class MainWindowUITests
{
    private Application? _app;
    private AutomationElement? _mainWindow;
    
    [TestInitialize]
    public void Setup()
    {
        _app = Application.Launch("CSG.RbVision.UI.WPF.exe");
        _mainWindow = _app.GetMainWindow(Automation);
    }
    
    [TestMethod]
    public void StartButton_ShouldChangeState()
    {
        var startButton = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("StartButton"));
        startButton.Click();
        
        // 验证状态改变
        var statusText = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("StatusText"));
        Assert.AreEqual("Running", statusText.Name);
    }
}
```

**性能测试**：
- [ ] 图像显示内存占用稳定
- [ ] 1000次结果更新无内存泄漏
- [ ] UI线程占用<20%

---

##<!-- CONTINUATION_MARKER_阶段6 -->
