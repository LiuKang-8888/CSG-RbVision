# CSG-RbVision 风格 UI 设计完整方案

> 参考海康机器人 CSG-RbVision 的界面设计理念，打造专业的工业视觉软件界面

---

## 📐 CSG-RbVision UI 核心设计理念

### 1. 整体布局风格

CSG-RbVision采用**工具箱+画布+属性**的经典三栏布局：

```
┌────────────────────────────────────────────────────────────────┐
│  [文件] [编辑] [视图] [工具] [运行] [帮助]                     │  菜单栏
├────────┬──────────────────────────────────────┬────────────────┤
│        │                                      │                │
│ 工具箱 │          图像显示 / 流程编辑区       │   属性/参数    │
│        │                                      │     面板       │
│ [定位] │                                      │                │
│ [测量] │        (可切换视图模式)              │  [选中工具的]  │
│ [检测] │                                      │  [详细参数]    │
│ [识别] │                                      │                │
│ [逻辑] │                                      │                │
│ [通信] │                                      │                │
│        │                                      │                │
│        │                                      │                │
├────────┴──────────────────────────────────────┴────────────────┤
│  [运行] [单步] [停止] │ 状态: 就绪 │ 耗时: 0ms │ 结果: -- │   │  控制栏
└────────────────────────────────────────────────────────────────┘
```

### 2. 核心特色功能

#### ✅ 拖拽式流程编辑
- 从工具箱拖拽工具到画布
- 自动生成数据流连接线
- 可视化查看每个步骤的结果

#### ✅ 实时参数调试
- 修改参数立即看到效果
- 支持参数微调滑块
- 阈值实时预览

#### ✅ 双模式切换
- **流程编辑模式**：设计检测流程
- **运行监控模式**：生产实时监控

---

## 🎨 详细界面设计方案

### 界面1: 主界面（流程编辑模式）

```
┌──────────────────────────────────────────────────────────────────────────┐
│ 文件(F) 编辑(E) 视图(V) 工具(T) 运行(R) 窗口(W) 帮助(H)                  │
├──────┬───────────────────────────────────────────────────────┬───────────┤
│工具箱│                  流程编辑区 (FlowCanvas)               │属性面板   │
│      │                                                        │           │
│ 🔍 定位│   ┌──────┐                                           │工具: 模板  │
│ ├─模板│   │Image │                                           │匹配       │
│ ├─边缘│   │Input │                                           │           │
│ ├─圆  │   └───┬──┘                                           │模板文件:  │
│ └─直线│       │                                              │[浏览...]  │
│       │       ▼                                              │           │
│ 📏 测量│   ┌──────────┐    ┌──────────┐                     │匹配阈值:  │
│ ├─卡尺│   │ Template │───▶│   Blob   │                     │░░░░░ 0.85 │
│ ├─圆  │   │ Matching │    │ Analysis │                     │           │
│ ├─点距│   └──────────┘    └────┬─────┘                     │搜索区域:  │
│ └─角度│                         │                            │X: [  0  ] │
│       │                         ▼                            │Y: [  0  ] │
│ 🔎 检测│                    ┌──────────┐                     │W: [ 1920] │
│ ├─Blob│                    │ Decision │                     │H: [ 1080] │
│ ├─缺陷│                    └────┬─────┘                     │           │
│ └─颜色│                         │                            │角度范围:  │
│       │                         ▼                            │[-180,180]│
│ 🔤 识别│                    [输出结果]                        │           │
│ ├─OCR │                                                      │☑️ 启用亚像│
│ ├─码  │  右键菜单:                                           │素精度     │
│ └─分类│  • 添加工具                                          │           │
│       │  • 删除工具                                          │[应用]     │
│ 🔧 逻辑│  • 禁用/启用                                         │[测试]     │
│ ├─运算│  • 查看结果                                          │           │
│ ├─判断│  • 设置断点                                          │执行结果:  │
│ └─循环│                                                      │❌ 未运行  │
│       │                                                      │           │
│ 📡 通信│                                                      │           │
│ ├─串口│                                                      │           │
│ ├─TCP │                                                      │           │
│ └─Modbus                                                     │           │
├──────┴───────────────────────────────────────────────────────┴───────────┤
│ ▶️ 运行 | ⏸️ 暂停 | ⏹️ 停止 | ⏭️ 单步 │ 🔄 重置 │ 状态: 就绪 │ 0ms │  │
└──────────────────────────────────────────────────────────────────────────┘
```

### 界面2: 图像显示与调试窗口

```
┌──────────────────────────────────────────────────────────────┐
│  图像窗口                                     [−][□][✕]        │
├──────────────────────────────────────────────────────────────┤
│ 📁 源图像 | 🔍 结果图像 | 📊 直方图 | 🎯 ROI设置             │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│                   [图像显示区域]                              │
│                                                              │
│         ┌──────────────────────────┐                        │
│         │                          │  ◄─ 模板匹配框         │
│         │      ★ 匹配点             │                        │
│         │      Score: 0.927        │                        │
│         │                          │                        │
│         └──────────────────────────┘                        │
│                                                              │
│                 ◉ Blob #1 (面积: 1250)                      │
│                 ◉ Blob #2 (面积: 980)                       │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ 🖱️ 平移 | 🔍+ 放大 | 🔍- 缩小 | 📐 测量 | 🎨 ROI | 💾 保存 │
│ 像素: (645, 382) | 灰度: 128 | RGB: (128,128,128) | 100%   │
└──────────────────────────────────────────────────────────────┘
```

### 界面3: 参数配置面板（详细版）

```
┌────────────────────────────────────────────┐
│ 模板匹配工具 - 参数配置          [?][✕]   │
├────────────────────────────────────────────┤
│                                            │
│ 📋 基本参数                                │
│ ┌────────────────────────────────────────┐ │
│ │ 模板图像:                              │ │
│ │ [C:\Templates\model.bmp]  [浏览...]   │ │
│ │                                        │ │
│ │ 匹配方法:                              │ │
│ │ [▼ 归一化相关性 (Normalized Correlation)] │ │
│ │                                        │ │
│ │ 匹配阈值: 0.85                         │ │
│ │ ├────────●────────┤ [0.00 - 1.00]     │ │
│ │                                        │ │
│ │ 预期数量: [1] 个  ☑️ 允许多个匹配      │ │
│ └────────────────────────────────────────┘ │
│                                            │
│ 🔍 搜索区域                                │
│ ┌────────────────────────────────────────┐ │
│ │ ◉ 整幅图像                             │ │
│ │ ○ 自定义区域                           │ │
│ │   X: [   0] Y: [   0]                 │ │
│ │   W: [1920] H: [1080]                 │ │
│ │                                        │ │
│ │ [🖱️ 在图像上绘制ROI]                   │ │
│ └────────────────────────────────────────┘ │
│                                            │
│ 🎯 高级选项                                │
│ ┌────────────────────────────────────────┐ │
│ │ ☑️ 启用角度搜索                         │ │
│ │   角度范围: [-10] ~ [+10] 度           │ │
│ │   角度步长: [1] 度                     │ │
│ │                                        │ │
│ │ ☑️ 启用缩放搜索                         │ │
│ │   缩放范围: [0.8] ~ [1.2]             │ │
│ │                                        │ │
│ │ ☑️ 金字塔加速                           │ │
│ │   金字塔层数: [3]                      │ │
│ │                                        │ │
│ │ ☑️ 亚像素精度                           │ │
│ └────────────────────────────────────────┘ │
│                                            │
│ 📊 结果输出                                │
│ ┌────────────────────────────────────────┐ │
│ │ ☑️ 输出位置 (X, Y)                     │ │
│ │ ☑️ 输出角度                             │ │
│ │ ☑️ 输出匹配分数                         │ │
│ │ ☑️ 输出缩放比例                         │ │
│ └────────────────────────────────────────┘ │
│                                            │
│          [确定]  [应用]  [取消]           │
└────────────────────────────────────────────┘
```

### 界面4: 运行监控模式

```
┌──────────────────────────────────────────────────────────────────────────┐
│ [流程编辑] 🔄 [运行监控]                                                  │
├─────────────────────────────────┬────────────────────────────────────────┤
│                                 │  ⚡ 实时统计                            │
│                                 │  ┌────────────────────────────────────┐│
│                                 │  │ 本班次统计 (08:00-16:00)           ││
│      [实时图像显示]              │  │                                    ││
│                                 │  │ 总数: ████████████ 1,234           ││
│    显示最新检测结果              │  │ 良品: ████████▓▓   1,210 (98.1%)  ││
│    + 叠加检测标注                │  │ 不良: ██▓▓▓▓▓▓▓▓     24 ( 1.9%)  ││
│                                 │  │                                    ││
│                                 │  │ 节拍: 320 ms/件                    ││
│  [检测区域]                      │  │ 合格率: 98.1%                      ││
│  [测量结果]                      │  └────────────────────────────────────┘│
│  [判定: OK]                      │                                        │
│                                 │  📊 实时趋势图                          │
│                                 │  ┌────────────────────────────────────┐│
│                                 │  │ 合格率 (%)                         ││
│                                 │  │ 100├─────────────────────          ││
│                                 │  │  98├───────▄▀▀▀▄▄────             ││
│                                 │  │  96├▄▄▄▄▄▀        ▀▄▄─            ││
│                                 │  │  94└─────────────────────          ││
│                                 │  │    8:00  10:00  12:00  14:00      ││
│                                 │  └────────────────────────────────────┘│
│                                 │                                        │
│                                 │  📋 最近结果 (最新100条)                │
│                                 │  ┌────────────────────────────────────┐│
│                                 │  │ 时间      结果  耗时   详情         ││
│                                 │  ├────────────────────────────────────┤│
│                                 │  │ 14:23:18  ✅ OK  310ms  正常        ││
│                                 │  │ 14:23:19  ✅ OK  305ms  正常        ││
│                                 │  │ 14:23:20  ❌ NG  340ms  尺寸超差    ││
│                                 │  │ 14:23:21  ✅ OK  315ms  正常        ││
│                                 │  │ 14:23:22  ✅ OK  308ms  正常        ││
│                                 │  │ ...                                ││
│                                 │  └────────────────────────────────────┘│
├─────────────────────────────────┴────────────────────────────────────────┤
│ ▶️ 运行中... │ 配方: PCB检测_v1.2 │ FPS: 3.1 │ 📊 统计 │ 💾 数据 │ ⚙️ │
└──────────────────────────────────────────────────────────────────────────┘
```

### 界面5: ROI与工具编辑器

```
┌────────────────────────────────────────────────────────────┐
│ ROI 编辑器                                      [−][□][✕]  │
├────────────────────────────────────────────────────────────┤
│ 🔧 工具: [▼ 矩形] [○ 圆形] [⬡ 多边形] [✏️ 自由绘制]        │
├────────────────────────────────────────────────────────────┤
│                                                            │
│                                                            │
│               ┌─────────────────────────┐                 │
│               │                         │  ◄── ROI #1     │
│               │        检测区域         │      矩形       │
│               │                         │      (活动)     │
│               │        ┌───┐            │                 │
│               │        │ + │ ◄────────────── 拖动控制点  │
│               │        └───┘            │                 │
│               │                         │                 │
│               └─────────────────────────┘                 │
│                                                            │
│                    ⭕ ROI #2 (圆形)                        │
│                                                            │
│                                                            │
├────────────────────────────────────────────────────────────┤
│ ROI列表:                           坐标信息:               │
│ ☑️ ROI #1 (矩形)  🎨 🗑️            X: 100   Y: 100         │
│ ☑️ ROI #2 (圆形)  🎨 🗑️            W: 500   H: 300         │
│ ☐ ROI #3 (多边形) 🎨 🗑️            角度: 0°                │
│                                                            │
│ [+ 新建ROI]  [导入]  [导出]        [确定]  [取消]         │
└────────────────────────────────────────────────────────────┘
```

---

## 🔧 核心组件实现方案

### 1. 流程编辑画布 (FlowCanvas)

📝 **CSG.RbVision.UI.WPF/Controls/FlowCanvas.xaml.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 使用WPF Canvas实现拖拽式编辑
// 2. 支持工具节点连线
// 3. 支持缩放、平移
// 4. 支持撤销/重做

public class FlowCanvas : Canvas
{
    private readonly List<ToolNode> _nodes = new();
    private readonly List<Connection> _connections = new();
    private Point _lastMousePosition;
    private ToolNode? _draggedNode;
    private bool _isPanning;
    
    // 缩放和平移
    private ScaleTransform _scaleTransform = new(1.0, 1.0);
    private TranslateTransform _translateTransform = new(0, 0);
    
    public FlowCanvas()
    {
        // 设置背景网格
        Background = CreateGridBrush();
        ClipToBounds = true;
        
        // 添加变换
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(_scaleTransform);
        transformGroup.Children.Add(_translateTransform);
        RenderTransform = transformGroup;
        
        // 事件绑定
        MouseDown += OnCanvasMouseDown;
        MouseMove += OnCanvasMouseMove;
        MouseUp += OnCanvasMouseUp;
        MouseWheel += OnCanvasMouseWheel;
        
        // 允许拖放
        AllowDrop = true;
        DragEnter += OnCanvasDragEnter;
        Drop += OnCanvasDrop;
    }
    
    // 从工具箱拖放工具到画布
    private void OnCanvasDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(typeof(ToolDescriptor)) is ToolDescriptor descriptor)
        {
            var position = e.GetPosition(this);
            
            // 创建新工具节点
            var node = new ToolNode
            {
                Id = Guid.NewGuid(),
                ToolType = descriptor.ToolType,
                DisplayName = descriptor.Name,
                Position = position,
                Width = 120,
                Height = 80
            };
            
            AddNode(node);
        }
    }
    
    public void AddNode(ToolNode node)
    {
        _nodes.Add(node);
        
        // 创建UI表示
        var visual = CreateNodeVisual(node);
        Children.Add(visual);
        
        SetLeft(visual, node.Position.X);
        SetTop(visual, node.Position.Y);
    }
    
    private FrameworkElement CreateNodeVisual(ToolNode node)
    {
        var border = new Border
        {
            Width = node.Width,
            Height = node.Height,
            Background = new SolidColorBrush(Color.FromRgb(60, 60, 80)),
            BorderBrush = Brushes.LightBlue,
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(4),
            Tag = node
        };
        
        var stackPanel = new StackPanel
        {
            Margin = new Thickness(8)
        };
        
        // 工具图标
        var icon = new TextBlock
        {
            Text = GetToolIcon(node.ToolType),
            FontSize = 24,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = Brushes.White
        };
        
        // 工具名称
        var name = new TextBlock
        {
            Text = node.DisplayName,
            FontSize = 12,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = Brushes.White,
            Margin = new Thickness(0, 4, 0, 0)
        };
        
        // 状态指示器
        var status = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = node.IsExecuted 
                ? (node.ExecutionSuccess ? Brushes.LimeGreen : Brushes.Red) 
                : Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 4, 0, 0)
        };
        
        stackPanel.Children.Add(icon);
        stackPanel.Children.Add(name);
        stackPanel.Children.Add(status);
        
        border.Child = stackPanel;
        
        // 绑定事件
        border.MouseDown += (s, e) =>
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _draggedNode = node;
                _lastMousePosition = e.GetPosition(this);
                e.Handled = true;
            }
        };
        
        // 右键菜单
        var contextMenu = new ContextMenu();
        contextMenu.Items.Add(CreateMenuItem("删除", () => RemoveNode(node)));
        contextMenu.Items.Add(CreateMenuItem("禁用", () => node.IsEnabled = !node.IsEnabled));
        contextMenu.Items.Add(CreateMenuItem("查看结果", () => ShowNodeResult(node)));
        border.ContextMenu = contextMenu;
        
        return border;
    }
    
    // 节点间连线绘制
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        
        // 绘制连接线
        var pen = new Pen(Brushes.White, 2);
        
        foreach (var connection in _connections)
        {
            var fromNode = _nodes.FirstOrDefault(n => n.Id == connection.FromNodeId);
            var toNode = _nodes.FirstOrDefault(n => n.Id == connection.ToNodeId);
            
            if (fromNode != null && toNode != null)
            {
                var start = new Point(
                    fromNode.Position.X + fromNode.Width / 2,
                    fromNode.Position.Y + fromNode.Height);
                    
                var end = new Point(
                    toNode.Position.X + toNode.Width / 2,
                    toNode.Position.Y);
                
                // 绘制贝塞尔曲线
                var bezier = CreateBezierCurve(start, end);
                dc.DrawGeometry(null, pen, bezier);
                
                // 绘制箭头
                DrawArrow(dc, pen, end, GetArrowAngle(start, end));
            }
        }
    }
    
    private PathGeometry CreateBezierCurve(Point start, Point end)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = start };
        
        var controlPoint1 = new Point(start.X, start.Y + (end.Y - start.Y) / 2);
        var controlPoint2 = new Point(end.X, start.Y + (end.Y - start.Y) / 2);
        
        figure.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, end, true));
        geometry.Figures.Add(figure);
        
        return geometry;
    }
    
    // 自动连接节点
    public void AutoConnect()
    {
        _connections.Clear();
        
        // 按Y坐标排序节点（从上到下）
        var sortedNodes = _nodes.OrderBy(n => n.Position.Y).ToList();
        
        for (int i = 0; i < sortedNodes.Count - 1; i++)
        {
            _connections.Add(new Connection
            {
                FromNodeId = sortedNodes[i].Id,
                ToNodeId = sortedNodes[i + 1].Id
            });
        }
        
        InvalidateVisual();
    }
    
    // 执行流程
    public async Task<WorkflowResult> ExecuteFlowAsync()
    {
        var sortedNodes = TopologicalSort(_nodes, _connections);
        var context = new WorkflowContext();
        
        foreach (var node in sortedNodes)
        {
            if (!node.IsEnabled)
                continue;
            
            node.IsExecuted = false;
            InvalidateVisual();
            
            try
            {
                var result = await ExecuteNodeAsync(node, context);
                node.ExecutionSuccess = result.Success;
                node.ExecutionResult = result;
                node.IsExecuted = true;
                
                // 更新UI
                Application.Current.Dispatcher.Invoke(() => InvalidateVisual());
                
                if (!result.Success && node.IsRequired)
                {
                    return new WorkflowResult { Success = false, FailedNode = node };
                }
            }
            catch (Exception ex)
            {
                node.ExecutionSuccess = false;
                node.IsExecuted = true;
                return new WorkflowResult { Success = false, ErrorMessage = ex.Message };
            }
        }
        
        return new WorkflowResult { Success = true };
    }
    
    private string GetToolIcon(ToolType toolType)
    {
        return toolType switch
        {
            ToolType.TemplateMatching => "🔍",
            ToolType.BlobAnalysis => "⭕",
            ToolType.Measurement => "📏",
            ToolType.OCR => "🔤",
            ToolType.Barcode => "▤",
            ToolType.DeepLearning => "🧠",
            _ => "🔧"
        };
    }
}

// 工具节点数据模型
public class ToolNode
{
    public Guid Id { get; set; }
    public ToolType ToolType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Point Position { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsExecuted { get; set; }
    public bool ExecutionSuccess { get; set; }
    public bool IsRequired { get; set; } = true;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public object? ExecutionResult { get; set; }
}

// 节点连接
public class Connection
{
    public Guid FromNodeId { get; set; }
    public Guid ToNodeId { get; set; }
    public string OutputPort { get; set; } = "Output";
    public string InputPort { get; set; } = "Input";
}

public enum ToolType
{
    ImageInput,
    TemplateMatching,
    BlobAnalysis,
    EdgeDetection,
    Measurement,
    OCR,
    Barcode,
    ColorDetection,
    DeepLearning,
    Logic,
    Output
}
```

### 2. 工具箱组件 (ToolBox)

📝 **CSG.RbVision.UI.WPF/Controls/ToolBoxPanel.xaml**

```xml
<UserControl x:Class="CSG.RbVision.UI.WPF.Controls.ToolBoxPanel"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Width="200">
    
    <Grid Background="#2D2D30">
        <ScrollViewer VerticalScrollBarVisibility="Auto">
            <StackPanel Margin="8">
                
                <!-- 定位工具组 -->
                <Expander Header="🔍 定位" IsExpanded="True" 
                         Style="{StaticResource ToolGroupExpander}">
                    <StackPanel>
                        <Button Content="模板匹配" Style="{StaticResource ToolButton}"
                               Tag="TemplateMatching" Click="OnToolClick"/>
                        <Button Content="边缘定位" Style="{StaticResource ToolButton}"
                               Tag="EdgeDetection" Click="OnToolClick"/>
                        <Button Content="圆查找" Style="{StaticResource ToolButton}"
                               Tag="CircleDetection" Click="OnToolClick"/>
                        <Button Content="直线查找" Style="{StaticResource ToolButton}"
                               Tag="LineDetection" Click="OnToolClick"/>
                    </StackPanel>
                </Expander>
                
                <!-- 测量工具组 -->
                <Expander Header="📏 测量" IsExpanded="True">
                    <StackPanel>
                        <Button Content="卡尺测量" Style="{StaticResource ToolButton}"
                               Tag="Caliper" Click="OnToolClick"/>
                        <Button Content="圆测量" Style="{StaticResource ToolButton}"
                               Tag="CircleMeasurement" Click="OnToolClick"/>
                        <Button Content="点到点距离" Style="{StaticResource ToolButton}"
                               Tag="PointDistance" Click="OnToolClick"/>
                        <Button Content="角度测量" Style="{StaticResource ToolButton}"
                               Tag="AngleMeasurement" Click="OnToolClick"/>
                    </StackPanel>
                </Expander>
                
                <!-- 检测工具组 -->
                <Expander Header="🔎 检测" IsExpanded="True">
                    <StackPanel>
                        <Button Content="Blob分析" Style="{StaticResource ToolButton}"
                               Tag="BlobAnalysis" Click="OnToolClick"/>
                        <Button Content="缺陷检测" Style="{StaticResource ToolButton}"
                               Tag="DefectDetection" Click="OnToolClick"/>
                        <Button Content="颜色识别" Style="{StaticResource ToolButton}"
                               Tag="ColorDetection" Click="OnToolClick"/>
                    </StackPanel>
                </Expander>
                
                <!-- 识别工具组 -->
                <Expander Header="🔤 识别" IsExpanded="True">
                    <StackPanel>
                        <Button Content="OCR识别" Style="{StaticResource ToolButton}"
                               Tag="OCR" Click="OnToolClick"/>
                        <Button Content="条码识别" Style="{StaticResource ToolButton}"
                               Tag="Barcode" Click="OnToolClick"/>
                        <Button Content="AI分类" Style="{StaticResource ToolButton}"
                               Tag="AIClassification" Click="OnToolClick"/>
                    </StackPanel>
                </Expander>
                
                <!-- 逻辑工具组 -->
                <Expander Header="🔧 逻辑" IsExpanded="False">
                    <StackPanel>
                        <Button Content="运算" Style="{StaticResource ToolButton}"
                               Tag="Calculation" Click="OnToolClick"/>
                        <Button Content="判断" Style="{StaticResource ToolButton}"
                               Tag="Condition" Click="OnToolClick"/>
                        <Button Content="循环" Style="{StaticResource ToolButton}"
                               Tag="Loop" Click="OnToolClick"/>
                    </StackPanel>
                </Expander>
                
                <!-- 通信工具组 -->
                <Expander Header="📡 通信" IsExpanded="False">
                    <StackPanel>
                        <Button Content="串口通信" Style="{StaticResource ToolButton}"
                               Tag="SerialPort" Click="OnToolClick"/>
                        <Button Content="TCP通信" Style="{StaticResource ToolButton}"
                               Tag="TCP" Click="OnToolClick"/>
                        <Button Content="Modbus" Style="{StaticResource ToolButton}"
                               Tag="Modbus" Click="OnToolClick"/>
                    </StackPanel>
                </Expander>
                
            </StackPanel>
        </ScrollViewer>
    </Grid>
</UserControl>
```

```csharp
// 工具箱后台代码
public partial class ToolBoxPanel : UserControl
{
    public event EventHandler<ToolDescriptor>? ToolSelected;
    
    public ToolBoxPanel()
    {
        InitializeComponent();
    }
    
    private void OnToolClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string toolTag)
        {
            var descriptor = new ToolDescriptor
            {
                ToolType = Enum.Parse<ToolType>(toolTag),
                Name = button.Content.ToString() ?? toolTag,
                Icon = GetToolIcon(toolTag)
            };
            
            // 启动拖拽操作
            DragDrop.DoDragDrop(button, descriptor, DragDropEffects.Copy);
            
            ToolSelected?.Invoke(this, descriptor);
        }
    }
    
    private string GetToolIcon(string toolTag)
    {
        return toolTag switch
        {
            "TemplateMatching" => "🔍",
            "BlobAnalysis" => "⭕",
            "OCR" => "🔤",
            _ => "🔧"
        };
    }
}

public class ToolDescriptor
{
    public ToolType ToolType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

### 3. 参数属性面板 (PropertyGrid)

📝 **CSG.RbVision.UI.WPF/Controls/PropertyGridPanel.xaml.cs**

```csharp
// ⚠️ 告诉Cursor：
// 1. 根据选中的工具动态生成参数UI
// 2. 参数修改实时反馈
// 3. 支持各种参数类型（数值、文件、枚举等）

public partial class PropertyGridPanel : UserControl
{
    private ToolNode? _selectedNode;
    
    public ToolNode? SelectedNode
    {
        get => _selectedNode;
        set
        {
            _selectedNode = value;
            RefreshProperties();
        }
    }
    
    private void RefreshProperties()
    {
        PropertiesContainer.Children.Clear();
        
        if (_selectedNode == null)
        {
            var emptyText = new TextBlock
            {
                Text = "请选择一个工具",
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            PropertiesContainer.Children.Add(emptyText);
            return;
        }
        
        // 工具标题
        AddSectionHeader($"{_selectedNode.DisplayName} - 参数配置");
        
        // 根据工具类型生成参数UI
        switch (_selectedNode.ToolType)
        {
            case ToolType.TemplateMatching:
                BuildTemplateMatchingProperties();
                break;
            case ToolType.BlobAnalysis:
                BuildBlobAnalysisProperties();
                break;
            case ToolType.OCR:
                BuildOCRProperties();
                break;
            // 其他工具类型...
        }
        
        // 执行结果显示
        if (_selectedNode.IsExecuted)
        {
            AddSectionHeader("执行结果");
            AddResultDisplay();
        }
    }
    
    private void BuildTemplateMatchingProperties()
    {
        // 基本参数组
        AddSectionHeader("📋 基本参数");
        
        // 模板文件选择
        AddFileParameter("模板图像", "TemplatePath", "图像文件|*.jpg;*.png;*.bmp");
        
        // 匹配方法
        AddEnumParameter("匹配方法", "MatchMethod", typeof(TemplateMatchModes), 
            TemplateMatchModes.CCoeffNormed);
        
        // 匹配阈值滑块
        AddSliderParameter("匹配阈值", "Threshold", 0.0, 1.0, 0.85, 0.01);
        
        // 预期数量
        AddIntParameter("预期数量", "ExpectedCount", 1, 1, 100);
        
        // 搜索区域组
        AddSectionHeader("🔍 搜索区域");
        
        AddCheckBoxParameter("使用整幅图像", "UseWholeImage", true);
        
        var roiGroup = new StackPanel { Margin = new Thickness(20, 0, 0, 0) };
        roiGroup.Children.Add(CreateIntInput("X坐标", "ROI_X", 0));
        roiGroup.Children.Add(CreateIntInput("Y坐标", "ROI_Y", 0));
        roiGroup.Children.Add(CreateIntInput("宽度", "ROI_Width", 1920));
        roiGroup.Children.Add(CreateIntInput("高度", "ROI_Height", 1080));
        
        var roiButton = new Button
        {
            Content = "🖱️ 在图像上绘制ROI",
            Margin = new Thickness(0, 4, 0, 0)
        };
        roiButton.Click += (s, e) => OpenROIEditor();
        roiGroup.Children.Add(roiButton);
        
        PropertiesContainer.Children.Add(roiGroup);
        
        // 高级选项组
        AddSectionHeader("🎯 高级选项");
        
        AddCheckBoxParameter("启用角度搜索", "EnableAngleSearch", false);
        AddSliderParameter("角度范围", "AngleRange", -180, 180, 10, 1);
        AddCheckBoxParameter("启用缩放搜索", "EnableScaleSearch", false);
        AddCheckBoxParameter("金字塔加速", "UsePyramid", true);
        AddCheckBoxParameter("亚像素精度", "SubPixelAccuracy", true);
        
        // 结果输出组
        AddSectionHeader("📊 结果输出");
        
        AddCheckBoxParameter("输出位置 (X, Y)", "OutputPosition", true);
        AddCheckBoxParameter("输出角度", "OutputAngle", true);
        AddCheckBoxParameter("输出匹配分数", "OutputScore", true);
    }
    
    private void AddFileParameter(string label, string paramKey, string filter)
    {
        var panel = new DockPanel { Margin = new Thickness(0, 4, 0, 4) };
        
        var labelText = new TextBlock 
        { 
            Text = label + ":", 
            Width = 100,
            VerticalAlignment = VerticalAlignment.Center
        };
        DockPanel.SetDock(labelText, Dock.Left);
        
        var browseButton = new Button
        {
            Content = "浏览...",
            Width = 60,
            Margin = new Thickness(4, 0, 0, 0)
        };
        DockPanel.SetDock(browseButton, Dock.Right);
        
        var textBox = new TextBox
        {
            IsReadOnly = true,
            Text = GetParameterValue(paramKey) as string ?? ""
        };
        
        browseButton.Click += (s, e) =>
        {
            var dialog = new OpenFileDialog { Filter = filter };
            if (dialog.ShowDialog() == true)
            {
                textBox.Text = dialog.FileName;
                SetParameterValue(paramKey, dialog.FileName);
            }
        };
        
        panel.Children.Add(labelText);
        panel.Children.Add(browseButton);
        panel.Children.Add(textBox);
        
        PropertiesContainer.Children.Add(panel);
    }
    
    private void AddSliderParameter(string label, string paramKey, 
        double min, double max, double defaultValue, double tickFrequency)
    {
        var panel = new StackPanel { Margin = new Thickness(0, 4, 0, 4) };
        
        var labelPanel = new DockPanel();
        var labelText = new TextBlock { Text = label + ":" };
        var valueText = new TextBlock 
        { 
            Text = defaultValue.ToString("F2"),
            HorizontalAlignment = HorizontalAlignment.Right,
            FontWeight = FontWeights.Bold
        };
        
        labelPanel.Children.Add(labelText);
        labelPanel.Children.Add(valueText);
        
        var slider = new Slider
        {
            Minimum = min,
            Maximum = max,
            Value = GetParameterValue(paramKey) as double? ?? defaultValue,
            TickFrequency = tickFrequency,
            IsSnapToTickEnabled = true,
            Margin = new Thickness(0, 2, 0, 0)
        };
        
        slider.ValueChanged += (s, e) =>
        {
            valueText.Text = e.NewValue.ToString("F2");
            SetParameterValue(paramKey, e.NewValue);
            
            // 实时预览
            if (AutoPreview)
            {
                TriggerPreview();
            }
        };
        
        panel.Children.Add(labelPanel);
        panel.Children.Add(slider);
        
        PropertiesContainer.Children.Add(panel);
    }
    
    private void AddCheckBoxParameter(string label, string paramKey, bool defaultValue)
    {
        var checkBox = new CheckBox
        {
            Content = label,
            IsChecked = GetParameterValue(paramKey) as bool? ?? defaultValue,
            Margin = new Thickness(0, 4, 0, 4)
        };
        
        checkBox.Checked += (s, e) => SetParameterValue(paramKey, true);
        checkBox.Unchecked += (s, e) => SetParameterValue(paramKey, false);
        
        PropertiesContainer.Children.Add(checkBox);
    }
    
    private void AddSectionHeader(string text)
    {
        var header = new TextBlock
        {
            Text = text,
            FontWeight = FontWeights.Bold,
            FontSize = 14,
            Margin = new Thickness(0, 12, 0, 8),
            Foreground = new SolidColorBrush(Color.FromRgb(0, 122, 204))
        };
        
        PropertiesContainer.Children.Add(header);
        PropertiesContainer.Children.Add(new Separator());
    }
    
    private object? GetParameterValue(string key)
    {
        return _selectedNode?.Parameters.ContainsKey(key) == true 
            ? _selectedNode.Parameters[key] 
            : null;
    }
    
    private void SetParameterValue(string key, object value)
    {
        if (_selectedNode != null)
        {
            _selectedNode.Parameters[key] = value;
        }
    }
    
    public bool AutoPreview { get; set; } = false;
    
    private async void TriggerPreview()
    {
        // 实时预览逻辑
        PreviewRequested?.Invoke(this, EventArgs.Empty);
    }
    
    public event EventHandler? PreviewRequested;
}
```

---

## 📱 附加功能界面

### 界面6: 数据统计与分析

```
┌────────────────────────────────────────────────────────────────┐
│ 数据统计                                           [−][□][✕]   │
├────────────────────────────────────────────────────────────────┤
│ 时间范围: [2026-11-23] 至 [2026-11-23]  班次: [全部▼]  [查询]│
├───────────────────────┬────────────────────────────────────────┤
│                       │                                        │
│  📊 生产概览           │   合格率趋势图                          │
│  ┌──────────────────┐ │   ┌────────────────────────────────┐  │
│  │ 总产量: 5,234    │ │   │100%├─────────────────          │  │
│  │ 良品: 5,129      │ │   │ 98%├───▄▀▀▀▄▄────             │  │
│  │ 不良: 105        │ │   │ 96%├▄▀      ▀▄▄─              │  │
│  │ 合格率: 98.0%    │ │   │ 94%└─────────────────          │  │
│  │                  │ │   │     8:00  12:00  16:00  20:00  │  │
│  │ 平均节拍: 318ms  │ │   └────────────────────────────────┘  │
│  │ 最快: 280ms      │ │                                        │
│  │ 最慢: 450ms      │ │   缺陷类型分布                          │
│  └──────────────────┘ │   ┌────────────────────────────────┐  │
│                       │   │                                │  │
│  📋 缺陷统计           │   │ 焊点缺失 ████████████ 45件    │  │
│  ┌──────────────────┐ │   │ 尺寸超差 ████████    30件     │  │
│  │ 类型     数量    │ │   │ 表面划痕 ████        15件     │  │
│  ├──────────────────┤ │   │ 其他     ███         15件     │  │
│  │ 焊点缺失   45    │ │   │                                │  │
│  │ 尺寸超差   30    │ │   └────────────────────────────────┘  │
│  │ 表面划痕   15    │ │                                        │
│  │ 其他       15    │ │   时段产量对比                          │
│  └──────────────────┘ │   ┌────────────────────────────────┐  │
│                       │   │  ▓▓▓▓▓▓▓▓ 08-10: 1,250         │  │
│  [导出Excel]          │   │  ▓▓▓▓▓▓▓▓▓ 10-12: 1,320       │  │
│  [导出PDF]            │   │  ▓▓▓▓▓▓▓ 12-14: 1,180         │  │
│  [打印]               │   │  ▓▓▓▓▓▓▓▓▓ 14-16: 1,484       │  │
│                       │   └────────────────────────────────┘  │
└───────────────────────┴────────────────────────────────────────┘
```

### 界面7: 相机与硬件配置

```
┌──────────────────────────────────────────────────────────┐
│ 硬件配置                                      [−][□][✕]  │
├──────────────────────────────────────────────────────────┤
│ 📷 相机配置              🔆 光源配置          🔌 IO配置  │
├──────────────────────────────────────────────────────────┤
│                                                          │
│ 相机列表:                                                 │
│ ┌────────────────────────────────────────────────────┐  │
│ │ ☑️ 相机1: Basler acA1920-40gm (已连接)             │  │
│ │   └ IP: 192.168.1.100                             │  │
│ │   └ 分辨率: 1920x1080                             │  │
│ │                                                    │  │
│ │ ☐ 相机2: HIK MV-CA016-10GM (未连接)              │  │
│ └────────────────────────────────────────────────────┘  │
│                                                          │
│ 相机参数:                                                 │
│ ┌────────────────────────────────────────────────────┐  │
│ │ 曝光时间(μs): [10000]    自动曝光: ☐              │  │
│ │ 增益: [1.2]              自动增益: ☐              │  │
│ │                                                    │  │
│ │ 触发模式: ◉ 软件触发  ○ 硬件触发  ○ 连续采集     │  │
│ │                                                    │  │
│ │ 触发源: [Line1 ▼]                                 │  │
│ │ 触发延迟(μs): [0]                                 │  │
│ │                                                    │  │
│ │ 像素格式: [Mono8 ▼]                               │  │
│ │                                                    │  │
│ │ [应用]  [测试采集]  [保存为默认]                  │  │
│ └────────────────────────────────────────────────────┘  │
│                                                          │
│ 光源控制器:                                               │
│ ┌────────────────────────────────────────────────────┐  │
│ │ 通道1: 环形光  ████████░░ 128  [测试]             │  │
│ │ 通道2: 背光    ██████░░░░ 100  [测试]             │  │
│ │ 通道3: 条形光  ██████████ 200  [测试]             │  │
│ │ 通道4: 未使用  ░░░░░░░░░░   0  [测试]             │  │
│ └────────────────────────────────────────────────────┘  │
│                                                          │
│          [确定]  [应用]  [取消]  [高级设置]             │
└──────────────────────────────────────────────────────────┘
```

---

## 🎯 实施关键点

### 1. 拖拽式编程体验
```csharp
// 实现要点：
- 工具箱 → 画布拖拽
- 画布上节点拖动重排
- 自动对齐网格
- 撤销/重做栈
```

### 2. 实时参数调试
```csharp
// 实现要点：
- 参数修改 → 触发预览
- 节流避免频繁计算
- 对比视图（原图/结果）
- 参数预设保存
```

### 3. 双模式无缝切换
```csharp
// 实现要点：
- 编辑模式：设计检测流程
- 运行模式：生产实时监控
- 一键切换，状态保持
```

### 4. 可视化结果叠加
```csharp
// 实现要点：
- 在原图上绘制检测结果
- 不同颜色标识OK/NG
- 半透明叠加层
- 可切换显示/隐藏
```

---

## 📦 完整主窗口代码框架

📝 **CSG.RbVision.UI.WPF/Views/MainWindow_CSG-RbVisionStyle.xaml**

```xml
<Window x:Class="CSG.RbVision.UI.WPF.Views.MainWindow_CSG-RbVisionStyle"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:controls="clr-namespace:CSG.RbVision.UI.WPF.Controls"
        Title="CSG-RbVision" Height="900" Width="1600"
        Background="#1E1E1E">
    
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- 菜单栏 -->
            <RowDefinition Height="Auto"/>  <!-- 工具栏 -->
            <RowDefinition Height="*"/>     <!-- 主工作区 -->
            <RowDefinition Height="Auto"/>  <!-- 状态栏 -->
        </Grid.RowDefinitions>
        
        <!-- 菜单栏 -->
        <Menu Grid.Row="0" Background="#2D2D30" Foreground="White">
            <MenuItem Header="文件(_F)">
                <MenuItem Header="新建项目" Icon="📄"/>
                <MenuItem Header="打开项目" Icon="📂"/>
                <MenuItem Header="保存项目" Icon="💾"/>
                <Separator/>
                <MenuItem Header="退出"/>
            </MenuItem>
            <MenuItem Header="编辑(_E)">
                <MenuItem Header="撤销" InputGestureText="Ctrl+Z"/>
                <MenuItem Header="重做" InputGestureText="Ctrl+Y"/>
            </MenuItem>
            <MenuItem Header="视图(_V)">
                <MenuItem Header="流程编辑模式" IsCheckable="True" IsChecked="True"/>
                <MenuItem Header="运行监控模式" IsCheckable="True"/>
            </MenuItem>
            <MenuItem Header="工���(_T)">
                <MenuItem Header="相机配置"/>
                <MenuItem Header="光源配置"/>
                <MenuItem Header="IO配置"/>
            </MenuItem>
            <MenuItem Header="运行(_R)">
                <MenuItem Header="运行" Icon="▶️" InputGestureText="F5"/>
                <MenuItem Header="停止" Icon="⏹️"/>
                <MenuItem Header="单步执行" Icon="⏭️" InputGestureText="F10"/>
            </MenuItem>
        </Menu>
        
        <!-- 工具栏 -->
        <ToolBarTray Grid.Row="1" Background="#2D2D30">
            <ToolBar>
                <Button Content="▶️ 运行" Command="{Binding RunCommand}"/>
                <Button Content="⏸️ 暂停" Command="{Binding PauseCommand}"/>
                <Button Content="⏹️ 停止" Command="{Binding StopCommand}"/>
                <Separator/>
                <Button Content="⏭️ 单步" Command="{Binding StepCommand}"/>
                <Button Content="🔄 重置" Command="{Binding ResetCommand}"/>
                <Separator/>
                <Button Content="💾 保存" Command="{Binding SaveCommand}"/>
                <Button Content="📂 打开" Command="{Binding OpenCommand}"/>
            </ToolBar>
        </ToolBarTray>
        
        <!-- 主工作区 -->
        <Grid Grid.Row="2">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="200"/>  <!-- 工具箱 -->
                <ColumnDefinition Width="4"/>
                <ColumnDefinition Width="*"/>     <!-- 画布 -->
                <ColumnDefinition Width="4"/>
                <ColumnDefinition Width="300"/>  <!-- 属性面板 -->
            </Grid.ColumnDefinitions>
            
            <!-- 工具箱 -->
            <controls:ToolBoxPanel Grid.Column="0" x:Name="ToolBox"/>
            
            <GridSplitter Grid.Column="1" HorizontalAlignment="Stretch"/>
            
            <!-- 中央画布区（TabControl切换视图） -->
            <TabControl Grid.Column="2" Background="#252526" BorderThickness="0">
                <!-- 流程编辑视图 -->
                <TabItem Header="流程编辑">
                    <controls:FlowCanvas x:Name="FlowCanvas"/>
                </TabItem>
                
                <!-- 图像显示视图 -->
                <TabItem Header="图像显示">
                    <controls:FastImageViewer x:Name="ImageViewer"/>
                </TabItem>
                
                <!-- 运行监控视图 -->
                <TabItem Header="运行监控">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="2*"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>
                        
                        <controls:FastImageViewer Grid.Column="0"/>
                        
                        <StackPanel Grid.Column="1" Margin="8">
                            <!-- 实时统计 -->
                        </StackPanel>
                    </Grid>
                </TabItem>
            </TabControl>
            
            <GridSplitter Grid.Column="3" HorizontalAlignment="Stretch"/>
            
            <!-- 属性面板 -->
            <controls:PropertyGridPanel Grid.Column="4" x:Name="PropertyGrid"/>
        </Grid>
        
        <!-- 状态栏 -->
        <StatusBar Grid.Row="3" Background="#007ACC" Foreground="White">
            <StatusBarItem>
                <TextBlock Text="{Binding SystemStatus}"/>
            </StatusBarItem>
            <Separator/>
            <StatusBarItem>
                <TextBlock Text="{Binding CurrentRecipe}"/>
            </StatusBarItem>
            <Separator/>
            <StatusBarItem>
                <StackPanel Orientation="Horizontal">
                    <TextBlock Text="耗时: "/>
                    <TextBlock Text="{Binding ExecutionTime}"/>
                    <TextBlock Text=" ms"/>
                </StackPanel>
            </StatusBarItem>
            <Separator/>
            <StatusBarItem>
                <StackPanel Orientation="Horizontal">
                    <TextBlock Text="结果: "/>
                    <TextBlock Text="{Binding LastResult}"/>
                </StackPanel>
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
```

---

## ✅ 总结：CSG-RbVision风格UI的核心价值

1. **降低使用门槛** - 拖拽式编程，无需编码
2. **提高开发效率** - 可视化流程，快速搭建
3. **增强调试体验** - 实时预览，参数可调
4. **专业视觉呈现** - 深色主题，工业风格
5. **双模式切换** - 编辑与运行分离

这套UI方案完全参考了CSG-RbVision的设计理念，可以让你的软件达到商业级水准！🎉
