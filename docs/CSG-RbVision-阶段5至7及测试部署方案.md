# CSG-RbVision - 阶段5至7完整方案

> 接续前文，完成阶段5-7及测试部署的详细实施计划

---

## 🎯 阶段5：WPF用户界面（续）

### 5.6 主窗口XAML实现（2天）

📝 **CSG.RbVision.UI.WPF/Views/MainWindow.xaml**

```xml
<!-- ⚠️ 告诉Cursor：
1. 使用ModernWpf主题
2. 布局采用Grid+DockPanel
3. 绑定使用{Binding}语法，命令使用{Binding XXXCommand}
-->

<Window x:Class="CSG.RbVision.UI.WPF.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:ui="http://schemas.modernwpf.com/2019"
        xmlns:controls="clr-namespace:CSG.RbVision.UI.WPF.Controls"
        xmlns:vm="clr-namespace:CSG.RbVision.UI.WPF.ViewModels"
        ui:WindowHelper.UseModernWindowStyle="True"
        Title="CSG-RbVision - 工业视觉检测系统" 
        Height="900" Width="1600"
        WindowState="Maximized">
    
    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>
    
    <Window.Resources>
        <!-- 状态指示器样式 -->
        <Style x:Key="StatusIndicator" TargetType="Ellipse">
            <Setter Property="Width" Value="12"/>
            <Setter Property="Height" Value="12"/>
            <Setter Property="Fill" Value="Gray"/>
            <Style.Triggers>
                <DataTrigger Binding="{Binding SystemState}" Value="Running">
                    <Setter Property="Fill" Value="LimeGreen"/>
                </DataTrigger>
                <DataTrigger Binding="{Binding SystemState}" Value="Error">
                    <Setter Property="Fill" Value="Red"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
        
        <!-- 统计卡片样式 -->
        <Style x:Key="StatCard" TargetType="Border">
            <Setter Property="Background" Value="{DynamicResource SystemControlBackgroundChromeMediumBrush}"/>
            <Setter Property="CornerRadius" Value="4"/>
            <Setter Property="Padding" Value="16"/>
            <Setter Property="Margin" Value="4"/>
        </Style>
    </Window.Resources>
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/> <!-- 工具栏 -->
            <RowDefinition Height="*"/>    <!-- 主内容区 -->
            <RowDefinition Height="Auto"/> <!-- 状态栏 -->
        </Grid.RowDefinitions>
        
        <!-- 顶部工具栏 -->
        <ui:CommandBar Grid.Row="0" DefaultLabelPosition="Right">
            <ui:AppBarButton Icon="Play" Label="启动" 
                           Command="{Binding StartCommand}"
                           IsEnabled="{Binding IsRunning, Converter={StaticResource InverseBoolConverter}}"/>
            <ui:AppBarButton Icon="Stop" Label="停止" 
                           Command="{Binding StopCommand}"
                           IsEnabled="{Binding IsRunning}"/>
            <ui:AppBarSeparator/>
            <ui:AppBarButton Icon="Target" Label="触发" 
                           Command="{Binding TriggerCommand}"
                           IsEnabled="{Binding IsRunning}"/>
            <ui:AppBarSeparator/>
            <ui:AppBarButton Icon="OpenFile" Label="加载配方" 
                           Command="{Binding LoadRecipeCommand}"/>
            <ui:AppBarButton Icon="Edit" Label="编辑配方" 
                           Command="{Binding EditRecipeCommand}"/>
            <ui:AppBarSeparator/>
            <ui:AppBarButton Icon="Setting" Label="设置" 
                           Command="{Binding OpenSettingsCommand}"/>
        </ui:CommandBar>
        
        <!-- 主内容区 -->
        <Grid Grid.Row="1">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="2*"/> <!-- 图像显示区 -->
                <ColumnDefinition Width="Auto"/> <!-- 分隔符 -->
                <ColumnDefinition Width="1*"/> <!-- 侧边栏 -->
            </Grid.ColumnDefinitions>
            
            <!-- 左侧：图像显示区 -->
            <Grid Grid.Column="0" Margin="8">
                <Grid.RowDefinitions>
                    <RowDefinition Height="*"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>
                
                <!-- 图像显示控件 -->
                <Border Grid.Row="0" BorderBrush="{DynamicResource SystemControlForegroundBaseMediumBrush}" 
                        BorderThickness="1">
                    <controls:FastImageViewer x:Name="ImageViewer"
                                            ImageSource="{Binding CurrentImage}"/>
                </Border>
                
                <!-- 图像信息栏 -->
                <StackPanel Grid.Row="1" Orientation="Horizontal" Margin="0,8,0,0">
                    <TextBlock Text="分辨率: "/>
                    <TextBlock Text="{Binding ImageWidth}"/>
                    <TextBlock Text=" x "/>
                    <TextBlock Text="{Binding ImageHeight}"/>
                    <TextBlock Text="  |  缩放: " Margin="16,0,0,0"/>
                    <TextBlock Text="{Binding ImageViewer.Scale, StringFormat={}{0:P0}}"/>
                </StackPanel>
            </Grid>
            
            <GridSplitter Grid.Column="1" Width="4" 
                        HorizontalAlignment="Stretch" 
                        Background="Transparent"/>
            
            <!-- 右侧：侧边栏 -->
            <Grid Grid.Column="2" Margin="8">
                <TabControl>
                    <!-- 实时统计标签页 -->
                    <TabItem Header="实时统计">
                        <ScrollViewer VerticalScrollBarVisibility="Auto">
                            <StackPanel Margin="8">
                                <!-- 状态信息 -->
                                <Border Style="{StaticResource StatCard}">
                                    <StackPanel>
                                        <TextBlock Text="系统状态" FontWeight="Bold" FontSize="14" Margin="0,0,0,8"/>
                                        <StackPanel Orientation="Horizontal">
                                            <Ellipse Style="{StaticResource StatusIndicator}" VerticalAlignment="Center"/>
                                            <TextBlock Text="{Binding SystemState}" Margin="8,0,0,0" FontSize="16"/>
                                        </StackPanel>
                                        <TextBlock Text="当前配方:" Margin="0,8,0,0"/>
                                        <TextBlock Text="{Binding CurrentRecipeName}" FontWeight="Bold"/>
                                    </StackPanel>
                                </Border>
                                
                                <!-- 生产统计 -->
                                <Border Style="{StaticResource StatCard}">
                                    <StackPanel>
                                        <TextBlock Text="本班次统计" FontWeight="Bold" FontSize="14" Margin="0,0,0,8"/>
                                        
                                        <Grid>
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*"/>
                                                <ColumnDefinition Width="*"/>
                                            </Grid.ColumnDefinitions>
                                            <Grid.RowDefinitions>
                                                <RowDefinition Height="Auto"/>
                                                <RowDefinition Height="Auto"/>
                                                <RowDefinition Height="Auto"/>
                                                <RowDefinition Height="Auto"/>
                                            </Grid.RowDefinitions>
                                            
                                            <TextBlock Grid.Row="0" Grid.Column="0" Text="总数:" Margin="0,4"/>
                                            <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding TotalCount}" 
                                                     FontWeight="Bold" HorizontalAlignment="Right"/>
                                            
                                            <TextBlock Grid.Row="1" Grid.Column="0" Text="良品:" Margin="0,4" Foreground="Green"/>
                                            <TextBlock Grid.Row="1" Grid.Column="1" Text="{Binding OkCount}" 
                                                     FontWeight="Bold" Foreground="Green" HorizontalAlignment="Right"/>
                                            
                                            <TextBlock Grid.Row="2" Grid.Column="0" Text="不良:" Margin="0,4" Foreground="Red"/>
                                            <TextBlock Grid.Row="2" Grid.Column="1" Text="{Binding NgCount}" 
                                                     FontWeight="Bold" Foreground="Red" HorizontalAlignment="Right"/>
                                            
                                            <TextBlock Grid.Row="3" Grid.Column="0" Text="合格率:" Margin="0,4"/>
                                            <TextBlock Grid.Row="3" Grid.Column="1" 
                                                     Text="{Binding YieldRate, StringFormat={}{0:F2}%}" 
                                                     FontWeight="Bold" FontSize="16" HorizontalAlignment="Right"/>
                                        </Grid>
                                    </StackPanel>
                                </Border>
                                
                                <!-- 性能指标 -->
                                <Border Style="{StaticResource StatCard}">
                                    <StackPanel>
                                        <TextBlock Text="性能指标" FontWeight="Bold" FontSize="14" Margin="0,0,0,8"/>
                                        <TextBlock Text="节拍时间:" Margin="0,4"/>
                                        <TextBlock Text="{Binding CycleTime, StringFormat={}{0:F0} ms}" 
                                                 FontWeight="Bold" FontSize="20"/>
                                    </StackPanel>
                                </Border>
                                
                                <!-- 最近结果列表 -->
                                <Border Style="{StaticResource StatCard}">
                                    <StackPanel>
                                        <TextBlock Text="最近结果" FontWeight="Bold" FontSize="14" Margin="0,0,0,8"/>
                                        <ListView ItemsSource="{Binding RecentResults}" 
                                                Height="300" 
                                                ScrollViewer.VerticalScrollBarVisibility="Auto">
                                            <ListView.View>
                                                <GridView>
                                                    <GridViewColumn Header="时间" Width="80">
                                                        <GridViewColumn.CellTemplate>
                                                            <DataTemplate>
                                                                <TextBlock Text="{Binding Timestamp, StringFormat=HH:mm:ss}"/>
                                                            </DataTemplate>
                                                        </GridViewColumn.CellTemplate>
                                                    </GridViewColumn>
                                                    <GridViewColumn Header="结果" Width="60">
                                                        <GridViewColumn.CellTemplate>
                                                            <DataTemplate>
                                                                <TextBlock Text="{Binding Decision}">
                                                                    <TextBlock.Style>
                                                                        <Style TargetType="TextBlock">
                                                                            <Style.Triggers>
                                                                                <DataTrigger Binding="{Binding Decision}" Value="OK">
                                                                                    <Setter Property="Foreground" Value="Green"/>
                                                                                    <Setter Property="FontWeight" Value="Bold"/>
                                                                                </DataTrigger>
                                                                                <DataTrigger Binding="{Binding Decision}" Value="NG">
                                                                                    <Setter Property="Foreground" Value="Red"/>
                                                                                    <Setter Property="FontWeight" Value="Bold"/>
                                                                                </DataTrigger>
                                                                            </Style.Triggers>
                                                                        </Style>
                                                                    </TextBlock.Style>
                                                                </TextBlock>
                                                            </DataTemplate>
                                                        </GridViewColumn.CellTemplate>
                                                    </GridViewColumn>
                                                    <GridViewColumn Header="耗时(ms)" Width="80">
                                                        <GridViewColumn.CellTemplate>
                                                            <DataTemplate>
                                                                <TextBlock Text="{Binding CycleTime, StringFormat={}{0:F0}}"/>
                                                            </DataTemplate>
                                                        </GridViewColumn.CellTemplate>
                                                    </GridViewColumn>
                                                </GridView>
                                            </ListView.View>
                                        </ListView>
                                    </StackPanel>
                                </Border>
                            </StackPanel>
                        </ScrollViewer>
                    </TabItem>
                    
                    <!-- 算法结果标签页 -->
                    <TabItem Header="算法详情">
                        <ScrollViewer VerticalScrollBarVisibility="Auto">
                            <ItemsControl ItemsSource="{Binding CurrentResult.StepResults}" Margin="8">
                                <ItemsControl.ItemTemplate>
                                    <DataTemplate>
                                        <Border Style="{StaticResource StatCard}">
                                            <StackPanel>
                                                <TextBlock Text="{Binding StepName}" FontWeight="Bold"/>
                                                <TextBlock Text="{Binding Success}" Margin="0,4"/>
                                                <TextBlock Text="{Binding Details}" Margin="0,4" TextWrapping="Wrap"/>
                                            </StackPanel>
                                        </Border>
                                    </DataTemplate>
                                </ItemsControl.ItemTemplate>
                            </ItemsControl>
                        </ScrollViewer>
                    </TabItem>
                    
                    <!-- 日志标签页 -->
                    <TabItem Header="系统日志">
                        <Grid>
                            <TextBox Text="{Binding SystemLog}" 
                                   IsReadOnly="True"
                                   VerticalScrollBarVisibility="Auto"
                                   FontFamily="Consolas"
                                   FontSize="11"
                                   Background="#1E1E1E"
                                   Foreground="White"
                                   Padding="8"/>
                        </Grid>
                    </TabItem>
                </TabControl>
            </Grid>
        </Grid>
        
        <!-- 底部状态栏 -->
        <StatusBar Grid.Row="2">
            <StatusBarItem>
                <StackPanel Orientation="Horizontal">
                    <Ellipse Style="{StaticResource StatusIndicator}"/>
                    <TextBlock Text="{Binding SystemState}" Margin="8,0,16,0"/>
                </StackPanel>
            </StatusBarItem>
            <Separator/>
            <StatusBarItem>
                <TextBlock Text="{Binding CurrentRecipeName}"/>
            </StatusBarItem>
            <Separator/>
            <StatusBarItem>
                <StackPanel Orientation="Horizontal">
                    <TextBlock Text="FPS: "/>
                    <TextBlock Text="{Binding CurrentFPS}"/>
                </StackPanel>
            </StatusBarItem>
            <StatusBarItem HorizontalAlignment="Right">
                <TextBlock Text="{Binding CurrentTime, StringFormat=yyyy-MM-dd HH:mm:ss}"/>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
```

### 5.7 App.xaml配置（1天）

📝 **CSG.RbVision.UI.WPF/App.xaml.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 使用Microsoft.Extensions.Hosting配置依赖注入
// 2. 配置全局异常处理
// 3. 初始化日志系统

public partial class App : Application
{
    private IHost? _host;
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // 配置依赖注入
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services, context.Configuration);
            })
            .Build();
        
        // 配置日志
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application starting");
        
        // 配置全局异常处理
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        
        // 配置GC
        ConfigureGarbageCollection();
        
        await _host.StartAsync();
        
        // 显示主窗口
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
    
    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // 配置
        services.AddSingleton<IConfiguration>(configuration);
        
        // 日志
        services.AddLogging(builder =>
        {
            builder.AddSerilog(LoggerSetup.CreateLogger(configuration));
        });
        
        // 核心服务
        services.AddSingleton<MatPool>();
        services.AddSingleton<ImageBufferPool>();
        
        // 硬件层
        services.AddSingleton<CameraFactory>();
        services.AddSingleton<ICameraManager, CameraManager>();
        services.AddSingleton<ILightController, MockLightController>();
        services.AddSingleton<IIOController, MockIOController>();
        
        // 业务层
        services.AddSingleton<VisionTaskStateMachine>();
        services.AddSingleton<WorkflowEngine>();
        services.AddSingleton<RecipeManager>();
        services.AddSingleton<VisionTaskExecutor>();
        
        // 数据层
        services.AddDbContext<VisionDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IResultRepository, ResultRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<RecipeEditorViewModel>();
        services.AddTransient<StatisticsViewModel>();
        
        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<RecipeEditorWindow>();
        services.AddTransient<StatisticsWindow>();
    }
    
    private void ConfigureGarbageCollection()
    {
        // 使用Server GC
        if (!GCSettings.IsServerGC)
        {
            MessageBox.Show("Warning: Server GC is not enabled. Performance may be affected.", 
                "GC Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        
        // 设置延迟模式
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }
    
    private void OnDispatcherUnhandledException(object sender, 
        DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = _host?.Services.GetService<ILogger<App>>();
        logger?.LogError(e.Exception, "Unhandled dispatcher exception");
        
        MessageBox.Show($"发生错误：{e.Exception.Message}\n\n详情已记录到日志文件。", 
            "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        
        e.Handled = true; // 防止程序崩溃
    }
    
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var logger = _host?.Services.GetService<ILogger<App>>();
        logger?.LogError(e.Exception, "Unobserved task exception");
        e.SetObserved();
    }
    
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var logger = _host?.Services.GetService<ILogger<App>>();
        var ex = e.ExceptionObject as Exception;
        logger?.LogCritical(ex, "Unhandled application exception");
        
        // 保存崩溃信息
        SaveCrashDump(ex);
    }
    
    private void SaveCrashDump(Exception? ex)
    {
        try
        {
            var dumpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                "Logs", $"crash_{DateTime.Now:yyyyMMddHHmmss}.txt");
            
            var info = new StringBuilder();
            info.AppendLine($"Crash Time: {DateTime.Now}");
            info.AppendLine($"Exception: {ex?.GetType().Name}");
            info.AppendLine($"Message: {ex?.Message}");
            info.AppendLine($"StackTrace:\n{ex?.StackTrace}");
            info.AppendLine($"\nOS: {Environment.OSVersion}");
            info.AppendLine($".NET: {Environment.Version}");
            info.AppendLine($"Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
            
            File.WriteAllText(dumpPath, info.ToString());
        }
        catch
        {
            // 忽略保存失败
        }
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        
        base.OnExit(e);
    }
}
```

---

## 🎯 阶段6：通信与集成（2周）

### 目标
实现MES集成、数据上报、IO控制等外部通信功能

### 6.1 数据仓储实现（2天）

📝 **CSG.RbVision.Data/VisionDbContext.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 使用EF Core 8
// 2. 配置实体关系
// 3. 添加种子数据

public class VisionDbContext : DbContext
{
    public DbSet<ResultEntity> Results { get; set; } = null!;
    public DbSet<ImageEntity> Images { get; set; } = null!;
    public DbSet<RecipeEntity> Recipes { get; set; } = null!;
    public DbSet<AlgorithmStepEntity> AlgorithmSteps { get; set; } = null!;
    
    public VisionDbContext(DbContextOptions<VisionDbContext> options) 
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 配置ResultEntity
        modelBuilder.Entity<ResultEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.RecipeName).HasMaxLength(100);
            entity.Property(e => e.Decision).IsRequired();
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Decision);
            
            // 一对多关系
            entity.HasMany(e => e.Images)
                .WithOne(i => i.Result)
                .HasForeignKey(i => i.ResultId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // 配置ImageEntity
        modelBuilder.Entity<ImageEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Decision).IsRequired();
            entity.HasIndex(e => e.ResultId);
        });
        
        // 配置RecipeEntity
        modelBuilder.Entity<RecipeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ProductModel).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            
            entity.HasMany(e => e.AlgorithmSteps)
                .WithOne(s => s.Recipe)
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // 配置AlgorithmStepEntity
        modelBuilder.Entity<AlgorithmStepEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.AlgorithmType).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Order).IsRequired();
        });
    }
}

// 实体类
public class ResultEntity
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public Decision Decision { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public long CaptureTimeMs { get; set; }
    public long ProcessingTimeMs { get; set; }
    public long TotalCycleTimeMs { get; set; }
    public string? MetricsJson { get; set; } // JSON存储详细指标
    
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();
}

public class ImageEntity
{
    public Guid Id { get; set; }
    public Guid ResultId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Decision Decision { get; set; }
    public DateTime SavedTime { get; set; }
    
    public ResultEntity Result { get; set; } = null!;
}

public class RecipeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductModel { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime ModifiedTime { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ConfigurationJson { get; set; } = string.Empty; // JSON存储完整配置
    
    public ICollection<AlgorithmStepEntity> AlgorithmSteps { get; set; } = new List<AlgorithmStepEntity>();
}

public class AlgorithmStepEntity
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AlgorithmType { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public string ParametersJson { get; set; } = string.Empty;
    
    public RecipeEntity Recipe { get; set; } = null!;
}
```

📝 **CSG.RbVision.Data/Repositories/ResultRepository.cs**

```csharp
public class ResultRepository : IResultRepository
{
    private readonly VisionDbContext _context;
    private readonly ILogger _logger;
    private readonly string _imageArchivePath;
    
    public ResultRepository(VisionDbContext context, ILogger<ResultRepository> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _imageArchivePath = configuration["ImageArchivePath"] ?? "Data/Images";
    }
    
    public async Task SaveResultAsync(VisionResult result)
    {
        try
        {
            var entity = new ResultEntity
            {
                Id = result.Id,
                Timestamp = result.Timestamp,
                RecipeName = result.RecipeName,
                Decision = result.Decision,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage,
                CaptureTimeMs = result.CaptureTimeMs,
                ProcessingTimeMs = result.ProcessingTimeMs,
                TotalCycleTimeMs = result.TotalCycleTimeMs,
                MetricsJson = JsonSerializer.Serialize(result.WorkflowResult)
            };
            
            // 保存图像（根据策略）
            if (ShouldSaveImage(result))
            {
                var imagePath = await SaveImageAsync(result.Image!, result.Decision);
                entity.Images.Add(new ImageEntity
                {
                    Id = Guid.NewGuid(),
                    ResultId = result.Id,
                    FilePath = imagePath,
                    Decision = result.Decision,
                    SavedTime = DateTime.Now
                });
            }
            
            await _context.Results.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save result");
            throw;
        }
    }
    
    private bool ShouldSaveImage(VisionResult result)
    {
        // 保存策略：NG全保存，OK按比例保存
        if (result.Decision == Decision.NG)
            return true;
        
        return Random.Shared.Next(100) < 10; // 10%的OK图像
    }
    
    private async Task<string> SaveImageAsync(ImageData image, Decision decision)
    {
        var date = DateTime.Now;
        var directory = Path.Combine(
            _imageArchivePath,
            date.ToString("yyyy"),
            date.ToString("MM"),
            date.ToString("dd"),
            decision.ToString());
        
        Directory.CreateDirectory(directory);
        
        var filename = $"{date:HHmmss}_{Guid.NewGuid():N}.jpg";
        var fullPath = Path.Combine(directory, filename);
        
        // 保存为JPEG（压缩）
        using var mat = image.ToMat();
        var encoded = Cv2.ImEncode(".jpg", mat, new ImageEncodingParam(ImwriteFlags.JpegQuality, 85));
        await File.WriteAllBytesAsync(fullPath, encoded);
        
        return fullPath;
    }
    
    public async Task<ProductionStatistics> GetStatisticsAsync(DateTime start, DateTime end)
    {
        var results = await _context.Results
            .Where(r => r.Timestamp >= start && r.Timestamp < end)
            .ToListAsync();
        
        return new ProductionStatistics
        {
            TotalCount = results.Count,
            OKCount = results.Count(r => r.Decision == Decision.OK),
            NGCount = results.Count(r => r.Decision == Decision.NG),
            AverageCycleTime = results.Any() ? results.Average(r => r.TotalCycleTimeMs) : 0
        };
    }
    
    public async Task<List<HourlyStatistic>> GetHourlyStatisticsAsync(DateTime start, DateTime end)
    {
        var results = await _context.Results
            .Where(r => r.Timestamp >= start && r.Timestamp < end)
            .ToListAsync();
        
        return results
            .GroupBy(r => new DateTime(r.Timestamp.Year, r.Timestamp.Month, r.Timestamp.Day, r.Timestamp.Hour, 0, 0))
            .Select(g => new HourlyStatistic
            {
                Hour = g.Key,
                TotalCount = g.Count(),
                OKCount = g.Count(r => r.Decision == Decision.OK),
                NGCount = g.Count(r => r.Decision == Decision.NG),
                YieldRate = g.Any() ? (double)g.Count(r => r.Decision == Decision.OK) / g.Count() * 100 : 0,
                AverageCycleTime = g.Average(r => r.TotalCycleTimeMs)
            })
            .OrderBy(s => s.Hour)
            .ToList();
    }
    
    public async Task<List<DefectStatistic>> GetDefectDistributionAsync(DateTime start, DateTime end)
    {
        // 从JSON中解析缺陷类型（需要根据实际数据结构调整）
        var ngResults = await _context.Results
            .Where(r => r.Timestamp >= start && r.Timestamp < end && r.Decision == Decision.NG)
            .ToListAsync();
        
        // 简化实现：按错误消息分组
        var grouped = ngResults
            .GroupBy(r => r.ErrorMessage ?? "Unknown")
            .Select(g => new DefectStatistic
            {
                DefectType = g.Key,
                Count = g.Count(),
                Percentage = (double)g.Count() / ngResults.Count * 100
            })
            .OrderByDescending(d => d.Count)
            .ToList();
        
        return grouped;
    }
}
```

### 6.2 MES集成服务（3天）

📝 **CSG.RbVision.Communication/MES/MESIntegrationService.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 支持HTTP REST API和MQTT两种通信方式
// 2. 实现断线重连和失败重试机制
// 3. 使用队列缓存待上报数据

public class MESIntegrationService : IDisposable
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly Channel<MESMessage> _messageQueue;
    private readonly CancellationTokenSource _cts;
    private Task? _uploadTask;
    
    // MQTT客户端（如果使用MQTT）
    private IMqttClient? _mqttClient;
    
    public MESIntegrationService(
        ILogger<MESIntegrationService> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        _cts = new CancellationTokenSource();
        
        _messageQueue = Channel.CreateUnbounded<MESMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
        
        // 配置HttpClient
        _httpClient.BaseAddress = new Uri(configuration["MES:BaseUrl"] ?? "http://localhost:5000");
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }
    
    public async Task StartAsync()
    {
        _logger.LogInformation("Starting MES integration service");
        
        var communicationType = _configuration["MES:Type"] ?? "HTTP";
        
        if (communicationType.Equals("MQTT", StringComparison.OrdinalIgnoreCase))
        {
            await InitializeMqttAsync();
        }
        
        // 启动上传任务
        _uploadTask = Task.Run(() => ProcessMessageQueueAsync(_cts.Token));
    }
    
    private async Task InitializeMqttAsync()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration["MES:MqttServer"], 
                int.Parse(_configuration["MES:MqttPort"] ?? "1883"))
            .WithClientId(_configuration["MES:DeviceId"])
            .WithCredentials(_configuration["MES:Username"], _configuration["MES:Password"])
            .WithCleanSession()
            .Build();
        
        _mqttClient.DisconnectedAsync += async e =>
        {
            _logger.LogWarning("MQTT disconnected, reconnecting...");
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            try
            {
                await _mqttClient.ConnectAsync(options);
                _logger.LogInformation("MQTT reconnected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT reconnection failed");
            }
        };
        
        await _mqttClient.ConnectAsync(options);
        
        // 订阅指令主题
        await _mqttClient.SubscribeAsync($"vision/{_configuration["MES:DeviceId"]}/commands");
        
        _mqttClient.ApplicationMessageReceivedAsync += OnMqttMessageReceivedAsync;
        
        _logger.LogInformation("MQTT client connected");
    }
    
    public async Task ReportProductionResultAsync(VisionResult result)
    {
        var message = new MESMessage
        {
            MessageType = MESMessageType.ProductionResult,
            Timestamp = result.Timestamp,
            Data = new
            {
                DeviceId = _configuration["MES:DeviceId"],
                ResultId = result.Id,
                RecipeName = result.RecipeName,
                Decision = result.Decision.ToString(),
                CycleTime = result.TotalCycleTimeMs,
                Timestamp = result.Timestamp
            }
        };
        
        await _messageQueue.Writer.WriteAsync(message);
    }
    
    public async Task ReportAlarmAsync(string alarmCode, string message)
    {
        var mesMessage = new MESMessage
        {
            MessageType = MESMessageType.Alarm,
            Timestamp = DateTime.Now,
            Data = new
            {
                DeviceId = _configuration["MES:DeviceId"],
                AlarmCode = alarmCode,
                Message = message,
                Timestamp = DateTime.Now
            }
        };
        
        await _messageQueue.Writer.WriteAsync(mesMessage);
    }
    
    private async Task ProcessMessageQueueAsync(CancellationToken cancellationToken)
    {
        await foreach (var message in _messageQueue.Reader.ReadAllAsync(cancellationToken))
        {
            var success = false;
            var retryCount = 0;
            const int maxRetries = 3;
            
            while (!success && retryCount < maxRetries)
            {
                try
                {
                    if (_mqttClient != null && _mqttClient.IsConnected)
                    {
                        await SendViaMqttAsync(message);
                    }
                    else
                    {
                        await SendViaHttpAsync(message);
                    }
                    
                    success = true;
                    _logger.LogDebug($"Message sent successfully: {message.MessageType}");
                }
                catch (Exception ex)
                {
                    retryCount++;
                    _logger.LogWarning(ex, $"Failed to send message (attempt {retryCount}/{maxRetries})");
                    
                    if (retryCount < maxRetries)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)), cancellationToken);
                    }
                }
            }
            
            if (!success)
            {
                _logger.LogError($"Message failed after {maxRetries} attempts: {message.MessageType}");
                // 可以选择保存到本地文件待后续重试
                await SaveFailedMessageAsync(message);
            }
        }
    }
    
    private async Task SendViaHttpAsync(MESMessage message)
    {
        var json = JsonSerializer.Serialize(message.Data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var endpoint = message.MessageType switch
        {
            MESMessageType.ProductionResult => "/api/vision/results",
            MESMessageType.Alarm => "/api/vision/alarms",
            MESMessageType.Heartbeat => "/api/vision/heartbeat",
            _ => throw new NotSupportedException($"Message type {message.MessageType} not supported")
        };
        
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task SendViaMqttAsync(MESMessage message)
    {
        if (_mqttClient == null || !_mqttClient.IsConnected)
            throw new InvalidOperationException("MQTT client not connected");
        
        var topic = message.MessageType switch
        {
            MESMessageType.ProductionResult => $"vision/{_configuration["MES:DeviceId"]}/results",
            MESMessageType.Alarm => $"vision/{_configuration["MES:DeviceId"]}/alarms",
            MESMessageType.Heartbeat => $"vision/{_configuration["MES:DeviceId"]}/heartbeat",
            _ => throw new NotSupportedException()
        };
        
        var json = JsonSerializer.Serialize(message.Data);
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(json)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        
        await _mqttClient.PublishAsync(mqttMessage);
    }
    
    private async Task OnMqttMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            _logger.LogInformation($"MQTT message received on {e.ApplicationMessage.Topic}: {payload}");
            
            var command = JsonSerializer.Deserialize<MESCommand>(payload);
            if (command != null)
            {
                await HandleMESCommandAsync(command);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process MQTT message");
        }
    }
    
    private async Task HandleMESCommandAsync(MESCommand command)
    {
        _logger.LogInformation($"Handling MES command: {command.Type}");
        
        switch (command.Type)
        {
            case "SWITCH_RECIPE":
                // 触发配方切换事件
                CommandReceived?.Invoke(this, new MESCommandEventArgs(command));
                break;
            
            case "START_PRODUCTION":
                CommandReceived?.Invoke(this, new MESCommandEventArgs(command));
                break;
            
            case "STOP_PRODUCTION":
                CommandReceived?.Invoke(this, new MESCommandEventArgs(command));
                break;
            
            default:
                _logger.LogWarning($"Unknown command type: {command.Type}");
                break;
        }
    }
    
    private async Task SaveFailedMessageAsync(MESMessage message)
    {
        try
        {
            var failedDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FailedMessages");
            Directory.CreateDirectory(failedDir);
            
            var filename = $"{message.Timestamp:yyyyMMddHHmmss}_{Guid.NewGuid():N}.json";
            var filePath = Path.Combine(failedDir, filename);
            
            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
            
            _logger.LogInformation($"Failed message saved to: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save failed message");
        }
    }
    
    public event EventHandler<MESCommandEventArgs>? CommandReceived;
    
    public void Dispose()
    {
        _cts.Cancel();
        _uploadTask?.Wait(TimeSpan.FromSeconds(5));
        _mqttClient?.DisconnectAsync().Wait();
        _mqttClient?.Dispose();
        _httpClient.Dispose();
        _cts.Dispose();
    }
}

public class MESMessage
{
    public MESMessageType MessageType { get; set; }
    public DateTime Timestamp { get; set; }
    public object Data { get; set; } = null!;
}

public enum MESMessageType
{
    ProductionResult,
    Alarm,
    Heartbeat
}

public class MESCommand
{
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class MESCommandEventArgs : EventArgs
{
    public MESCommand Command { get; }
    
    public MESCommandEventArgs(MESCommand command)
    {
        Command = command;
    }
}
```

### 6.3 Modbus通信（2天）

📝 **CSG.RbVision.Communication/Modbus/ModbusController.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 使用NModbus4库
// 2. 支持Modbus TCP和RTU
// 3. 实现读写线圈、保持寄存器

public class ModbusController : IIOController
{
    private readonly ILogger _logger;
    private IModbusMaster? _modbusMaster;
    private TcpClient? _tcpClient;
    private SerialPort? _serialPort;
    private readonly string _connectionType;
    private readonly SemaphoreSlim _accessLock = new(1, 1);
    
    public ModbusController(ILogger<ModbusController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionType = configuration["Modbus:Type"] ?? "TCP";
    }
    
    public async Task<Result<bool>> ConnectAsync(string address)
    {
        try
        {
            if (_connectionType.Equals("TCP", StringComparison.OrdinalIgnoreCase))
            {
                var parts = address.Split(':');
                var host = parts[0];
                var port = parts.Length > 1 ? int.Parse(parts[1]) : 502;
                
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(host, port);
                
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateMaster(_tcpClient);
                
                _logger.LogInformation($"Modbus TCP connected to {address}");
            }
            else // RTU
            {
                _serialPort = new SerialPort(address, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Open();
                
                var factory = new ModbusFactory();
                _modbusMaster = factory.CreateRtuMaster(_serialPort);
                
                _logger.LogInformation($"Modbus RTU connected to {address}");
            }
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to connect Modbus: {address}");
            return Result<bool>.Failure(ErrorCode.ConnectionFailed, ex.Message);
        }
    }
    
    public async Task<Result<bool>> DisconnectAsync()
    {
        try
        {
            _tcpClient?.Close();
            _serialPort?.Close();
            _modbusMaster = null;
            
            _logger.LogInformation("Modbus disconnected");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect Modbus");
            return Result<bool>.Failure(ErrorCode.DisconnectionFailed, ex.Message);
        }
    }
    
    public async Task<Result<bool>> WriteDigitalOutputAsync(int address, bool value)
    {
        await _accessLock.WaitAsync();
        
        try
        {
            if (_modbusMaster == null)
                return Result<bool>.Failure(ErrorCode.NotConnected, "Modbus not connected");
            
            await Task.Run(() => _modbusMaster.WriteSingleCoil(1, (ushort)address, value));
            
            _logger.LogDebug($"Wrote coil {address} = {value}");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to write coil {address}");
            return Result<bool>.Failure(ErrorCode.WriteFailed, ex.Message);
        }
        finally
        {
            _accessLock.Release();
        }
    }
    
    public async Task<Result<bool>> ReadDigitalInputAsync(int address)
    {
        await _accessLock.WaitAsync();
        
        try
        {
            if (_modbusMaster == null)
                return Result<bool>.Failure(ErrorCode.NotConnected, "Modbus not connected");
            
            var coils = await Task.Run(() => _modbusMaster.ReadCoils(1, (ushort)address, 1));
            var value = coils[0];
            
            _logger.LogDebug($"Read coil {address} = {value}");
            return Result<bool>.Success(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to read coil {address}");
            return Result<bool>.Failure(ErrorCode.ReadFailed, ex.Message);
        }
        finally
        {
            _accessLock.Release();
        }
    }
    
    public async Task<Result<ushort>> ReadHoldingRegisterAsync(int address)
    {
        await _accessLock.WaitAsync();
        
        try
        {
            if (_modbusMaster == null)
                return Result<ushort>.Failure(ErrorCode.NotConnected, "Modbus not connected");
            
            var registers = await Task.Run(() => _modbusMaster.ReadHoldingRegisters(1, (ushort)address, 1));
            var value = registers[0];
            
            _logger.LogDebug($"Read holding register {address} = {value}");
            return Result<ushort>.Success(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to read holding register {address}");
            return Result<ushort>.Failure(ErrorCode.ReadFailed, ex.Message);
        }
        finally
        {
            _accessLock.Release();
        }
    }
    
    public async Task<Result<bool>> WriteHoldingRegisterAsync(int address, ushort value)
    {
        await _accessLock.WaitAsync();
        
        try
        {
            if (_modbusMaster == null)
                return Result<bool>.Failure(ErrorCode.NotConnected, "Modbus not connected");
            
            await Task.Run(() => _modbusMaster.WriteSingleRegister(1, (ushort)address, value));
            
            _logger.LogDebug($"Wrote holding register {address} = {value}");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to write holding register {address}");
            return Result<bool>.Failure(ErrorCode.WriteFailed, ex.Message);
        }
        finally
        {
            _accessLock.Release();
        }
    }
}
```

### 阶段6测试标准

**单元测试**：
```csharp
public class MESIntegrationServiceTests
{
    [Fact]
    public async Task ReportResult_ShouldQueueMessage()
    {
        // 测试消息入队
    }
    
    [Fact]
    public async Task SendViaHttp_ShouldRetryOnFailure()
    {
        // 测试失败重试
    }
}

public class ModbusControllerTests
{
    [Fact]
    public async Task WriteCoil_ShouldSucceed()
    {
        // 需要Modbus模拟器
    }
}
```

**集成测试**（需要外部环境）：
- [ ] MES HTTP接口正常上报
- [ ] MQTT连接、发布、订阅正常
- [ ] Modbus TCP/RTU读写正常
- [ ] 断线自动重连

---

## 🎯 阶段7：测试与优化（2-3周）

### 目标
全面测试、性能优化、稳定性验证

### 7.1 性能测试框架（2天）

📝 **CSG.RbVision.PerformanceTests/PerformanceTestBase.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 使用BenchmarkDotNet进行性能测试
// 2. 测试内存分配和GC情况
// 3. 生成性能报告

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ImageProcessingBenchmarks
{
    private ImageData _testImage = null!;
    private Mat _testMat = null!;
    private MatPool _matPool = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _testImage = ImageData.FromFile("TestData/Images/test.jpg");
        _testMat = Cv2.ImRead("TestData/Images/test.jpg");
        _matPool = new MatPool(new Size(1920, 1080), MatType.CV_8UC1, 5);
    }
    
    [Benchmark(Baseline = true)]
    public void DirectMatCreation()
    {
        using var mat = new Mat(1920, 1080, MatType.CV_8UC1);
        mat.SetTo(Scalar.All(0));
    }
    
    [Benchmark]
    public void MatPoolRent()
    {
        var mat = _matPool.Rent();
        _matPool.Return(mat);
    }
    
    [Benchmark]
    public void GaussianBlur_WithoutPool()
    {
        using var src = _testMat.Clone();
        using var dst = new Mat();
        Cv2.GaussianBlur(src, dst, new Size(5, 5), 0);
    }
    
    [Benchmark]
    public void GaussianBlur_WithPool()
    {
        var dst = _matPool.Rent();
        Cv2.GaussianBlur(_testMat, dst, new Size(5, 5), 0);
        _matPool.Return(dst);
    }
    
    [GlobalCleanup]
    public void Cleanup()
    {
        _testImage?.Dispose();
        _testMat?.Dispose();
        _matPool?.Dispose();
    }
}

[MemoryDiagnoser]
public class WorkflowBenchmarks
{
    private WorkflowEngine _engine = null!;
    private Workflow _workflow = null!;
    private ImageData _testImage = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        // 配置依赖注入
        var services = new ServiceCollection();
        services.AddLogging();
        // ... 添加其他服务
        
        var serviceProvider = services.BuildServiceProvider();
        _engine = serviceProvider.GetRequiredService<WorkflowEngine>();
        
        _workflow = new Workflow
        {
            Name = "BenchmarkWorkflow",
            Steps = new List<WorkflowStep>
            {
                new() { Name = "Step1", StepType = StepType.Algorithm, /* ... */ },
                new() { Name = "Step2", StepType = StepType.Algorithm, /* ... */ }
            }
        };
        
        _testImage = ImageData.FromFile("TestData/Images/test.jpg");
    }
    
    [Benchmark]
    public async Task ExecuteWorkflow()
    {
        var result = await _engine.ExecuteAsync(_workflow, _testImage);
    }
    
    [GlobalCleanup]
    public void Cleanup()
    {
        _testImage?.Dispose();
    }
}

// 运行性能测试
// dotnet run -c Release --project CSG.RbVision.PerformanceTests
```

### 7.2 稳定性测试（3天）

📝 **CSG.RbVision.IntegrationTests/StabilityTests.cs**

```csharp
public class StabilityTests
{
    private VisionTaskExecutor _executor = null!;
    private MockCamera _camera = null!;
    
    [Fact(Timeout = 90000)] // 90秒超时
    public async Task ContinuousExecution_ShouldRunFor1000Cycles()
    {
        // Arrange
        _camera = new MockCamera("TestData/Images");
        // ... 初始化executor
        
        var completedCount = 0;
        var failedCount = 0;
        
        _executor.ResultProcessed += (s, result) =>
        {
            if (result.Success)
                Interlocked.Increment(ref completedCount);
            else
                Interlocked.Increment(ref failedCount);
        };
        
        // Act
        await _executor.StartAsync();
        
        for (int i = 0; i < 1000; i++)
        {
            _executor.Trigger();
            await Task.Delay(10); // 100fps
        }
        
        // 等待所有任务完成
        await Task.Delay(5000);
        await _executor.StopAsync();
        
        // Assert
        Assert.True(completedCount >= 950, $"Completed: {completedCount}/1000");
        Assert.True(failedCount < 50, $"Failed: {failedCount}");
    }
    
    [Fact(Timeout = 300000)] // 5分钟超时
    public async Task LongRunning_ShouldNotLeakMemory()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        
        // Act
        await _executor.StartAsync();
        
        for (int i = 0; i < 5000; i++)
        {
            _executor.Trigger();
            await Task.Delay(10);
            
            if (i % 1000 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                var currentMemory = GC.GetTotalMemory(false);
                var growth = (currentMemory - initialMemory) / 1024.0 / 1024.0;
                
                Assert.True(growth < 100, $"Memory growth: {growth:F2} MB at cycle {i}");
            }
        }
        
        await _executor.StopAsync();
        
        // Assert
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var finalMemory = GC.GetTotalMemory(false);
        var totalGrowth = (finalMemory - initialMemory) / 1024.0 / 1024.0;
        
        Assert.True(totalGrowth < 200, $"Total memory growth: {totalGrowth:F2} MB");
    }
    
    [Theory]
    [InlineData(10)]  // 10个并发任务
    [InlineData(50)]  // 50个并发任务
    public async Task ConcurrentExecution_ShouldHandleLoad(int concurrency)
    {
        // 测试并发处理能力
        var tasks = new List<Task>();
        
        for (int i = 0; i < concurrency; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                for (int j = 0; j < 100; j++)
                {
                    _executor.Trigger();
                    await Task.Delay(Random.Shared.Next(5, 20));
                }
            }));
        }
        
        await Task.WhenAll(tasks);
    }
}
```

### 7.3 性能优化清单（2天）

**内存优化**：
- [ ] 确认所有Mat对象使用using或对象池
- [ ] 检查大对象堆（LOH）分配
- [ ] 验证无内存泄漏（使用dotMemory或PerfView）
- [ ] 优化图像Buffer复用

**CPU优化**：
- [ ] 算法并行化（Parallel.ForEach）
- [ ] SIMD优化（System.Numerics.Vectors）
- [ ] 减少不必要的图像拷贝
- [ ] 使用Span<T>减少分配

**GC优化**：
- [ ] 确认Server GC已启用
- [ ] 设置SustainedLowLatency模式
- [ ] 减少Gen0/Gen1 GC频率
- [ ] 避免大对象分配

**UI优化**：
- [ ] 图像显示使用后台线程转换
- [ ] WriteableBitmap复用
- [ ] 使用虚拟化控件显示大列表
- [ ] 减少UI线程绑定更新频率

### 7.4 问题诊断工具集成（1天）

📝 **CSG.RbVision.Diagnostics/DiagnosticService.cs**

```csharp
public class DiagnosticService
{
    private readonly ILogger _logger;
    private readonly PerformanceCounter _cpuCounter;
    private readonly PerformanceCounter _memCounter;
    
    public DiagnosticService(ILogger<DiagnosticService> logger)
    {
        _logger = logger;
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _memCounter = new PerformanceCounter("Memory", "Available MBytes");
    }
    
    public SystemDiagnostics GetSystemDiagnostics()
    {
        var process = Process.GetCurrentProcess();
        
        return new SystemDiagnostics
        {
            // CPU
            CpuUsage = _cpuCounter.NextValue(),
            
            // 内存
            ProcessMemoryMB = process.WorkingSet64 / 1024 / 1024,
            AvailableMemoryMB = _memCounter.NextValue(),
            ManagedMemoryMB = GC.GetTotalMemory(false) / 1024 / 1024,
            
            // GC
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            
            // 线程
            ThreadCount = process.Threads.Count,
            HandleCount = process.HandleCount,
            
            // 运行时间
            Uptime = DateTime.Now - process.StartTime
        };
    }
    
    public async Task GenerateDiagnosticReportAsync(string outputPath)
    {
        var report = new StringBuilder();
        
        report.AppendLine("=== CSG-RbVision Diagnostic Report ===");
        report.AppendLine($"Generated: {DateTime.Now}");
        report.AppendLine();
        
        // 系统信息
        report.AppendLine("## System Information");
        report.AppendLine($"OS: {Environment.OSVersion}");
        report.AppendLine($".NET: {Environment.Version}");
        report.AppendLine($"Processor Count: {Environment.ProcessorCount}");
        report.AppendLine($"Server GC: {GCSettings.IsServerGC}");
        report.AppendLine($"GC Latency Mode: {GCSettings.LatencyMode}");
        report.AppendLine();
        
        // 性能指标
        var diag = GetSystemDiagnostics();
        report.AppendLine("## Performance Metrics");
        report.AppendLine($"CPU Usage: {diag.CpuUsage:F2}%");
        report.AppendLine($"Process Memory: {diag.ProcessMemoryMB} MB");
        report.AppendLine($"Managed Memory: {diag.ManagedMemoryMB} MB");
        report.AppendLine($"Available Memory: {diag.AvailableMemoryMB} MB");
        report.AppendLine($"Thread Count: {diag.ThreadCount}");
        report.AppendLine($"GC Collections: Gen0={diag.Gen0Collections}, Gen1={diag.Gen1Collections}, Gen2={diag.Gen2Collections}");
        report.AppendLine($"Uptime: {diag.Uptime}");
        report.AppendLine();
        
        // 配置信息
        report.AppendLine("## Configuration");
        // ... 添加配置信息
        
        await File.WriteAllTextAsync(outputPath, report.ToString());
        _logger.LogInformation($"Diagnostic report saved to: {outputPath}");
    }
}

public class SystemDiagnostics
{
    public float CpuUsage { get; set; }
    public long ProcessMemoryMB { get; set; }
    public float AvailableMemoryMB { get; set; }
    public long ManagedMemoryMB { get; set; }
    public int Gen0Collections { get; set; }
    public int Gen1Collections { get; set; }
    public int Gen2Collections { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public TimeSpan Uptime { get; set; }
}
```

### 阶段7测试清单

**功能测试**：
- [ ] 所有算法正确性验证
- [ ] 配方加载和切换
- [ ] 硬件连接和断线处理
- [ ] MES通信正常
- [ ] IO信号正确

**性能测试**：
- [ ] 单张图像处理<500ms
- [ ] 连续1000张无性能衰减
- [ ] 内存增长<100MB/小时
- [ ] CPU占用<50%
- [ ] UI响应<100ms

**稳定性测试**：
- [ ] 24小时连续运行无崩溃
- [ ] 10000次循环无泄漏
- [ ] 相机断线恢复
- [ ] MES断线重连
- [ ] 配方切换无异常

**兼容性测试**：
- [ ] Windows 10 1809+
- [ ] Windows 11
- [ ] 不同分辨率屏幕
- [ ] 不同相机型号

---

## 📦 部署方案

### 部署清单

**方式1：自包含部署（推荐）**

```powershell
# 发布脚本
dotnet publish src/CSG.RbVision.UI.WPF/CSG.RbVision.UI.WPF.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishReadyToRun=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o publish/CSG-RbVision
```

**目录结构**：
```
CSG-RbVision/
├── CSG.RbVision.exe              # 主程序
├── CSG.RbVision.dll
├── appsettings.json              # 配置文件
├── Recipes/                      # 配方目录
├── TestData/                     # 测试数据
│   └── Images/
├── Logs/                         # 日志目录
├── Data/                         # 数据库目录
│   └── vision.db
├── ThirdParty/                   # 第三方库
│   ├── Basler/
│   └── Hikvision/
└── README.txt                    # 使用说明
```

**安装包制作（使用Inno Setup）**：

📝 **installer.iss**

```inno
#define MyAppName "CSG-RbVision"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Your Company"
#define MyAppExeName "CSG.RbVision.exe"

[Setup]
AppId={{YOUR-GUID-HERE}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=installer
OutputBaseFilename=CSG-RbVision_Setup_v{#MyAppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

[Languages]
Name: "chinesesimplified"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "附加图标:"

[Files]
Source: "publish\CSG-RbVision\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "启动 {#MyAppName}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
begin
  // 检查.NET运行时（如果不是自包含部署）
  Result := True;
end;
```

---

## 📚 文档输出清单

### 需要编写的文档

1. **用户手册**（`docs/UserManual.md`）
   - 软件安装指南
   - 界面功能说明
   - 配方配置教程
   - 常见问题解答

2. **开发文档**（`docs/Developer.md`）
   - 架构设计说明
   - API参考
   - 插件开发指南
   - 代码规范

3. **部署文档**（`docs/Deployment.md`）
   - 系统要求
   - 安装步骤
   - 配置说明
   - 升级指南

4. **测试报告**（`docs/TestReport.md`）
   - 功能测试结果
   - 性能测试数据
   - 稳定性测试报告

---

## 🎯 验收标准

### 最终验收检查清单

**功能完整性**：
- [ ] 所有计划功能已实现
- [ ] 所有测试用例通过
- [ ] 无严重Bug
- [ ] 文档齐全

**性能指标**：
- [ ] 处理速度达标（<500ms）
- [ ] 准确率达标（>99.5%）
- [ ] 内存稳定（<2GB）
- [ ] CPU占用合理（<50%）

**稳定性**：
- [ ] 24小时稳定运行
- [ ] 无内存泄漏
- [ ] 异常自动恢复

**代码质量**：
- [ ] 代码覆盖率>80%
- [ ] 无严重代码坏味道
- [ ] 遵循编码规范

**用户体验**：
- [ ] 界面友好易用
- [ ] 操作响应及时
- [ ] 错误提示清晰

---

## 🚀 后续优化方向

1. **功能扩展**
   - 支持更多算法类型
   - 3D视觉支持
   - 机器学习模型训练集成

2. **性能提升**
   - GPU加速优化
   - 分布式处理
   - 更激进的并行化

3. **易用性改进**
   - 拖拽式流程编辑器
   - 在线算法调试工具
   - 自动参数优化

4. **云端集成**
   - 云端配方管理
   - 远程诊断升级
   - 大数据分析平台

---

至此，CSG-RbVision的完整落地方案已全部完成！✅
