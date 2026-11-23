# CSG-RbVision完整落地方案

> **项目代号**: CSG-RbVision  
> **技术栈**: C# + .NET 8 + WPF + OpenCvSharp4  
> **目标**: 构建高性能、可扩展的工业视觉检测平台  
> **预计工期**: 17-21周（4-5个月）

---

## 📋 目录

- [一、项目概览](#一项目概览)
- [二、技术架构](#二技术架构)
- [三、开发环境准备](#三开发环境准备)
- [四、分阶段实施计划](#四分阶段实施计划)
- [五、测试策略](#五测试策略)
- [六、Cursor使用指南](#六cursor使用指南)
- [七、风险与应对](#七风险与应对)

---

## 一、项目概览

### 1.1 核心目标

构建一个企业级工业视觉软件平台，具备以下核心能力：

- ✅ 多相机并发采集与处理
- ✅ 灵活的算法插件化架构
- ✅ 配方管理与快速切换
- ✅ MES/ERP系统集成
- ✅ 实时统计与SPC分析
- ✅ 远程监控与诊断

### 1.2 技术选型确定

```yaml
核心技术栈:
  框架: .NET 8 LTS (支持到2026年11月)
  语言: C# 12
  UI框架: WPF + ModernWpfUI
  图像处理: OpenCvSharp4 4.9.0+
  深度学习: ONNX Runtime 1.17+
  数据库: SQLite (开发) / SQL Server (生产)
  日志: Serilog
  IoC容器: Microsoft.Extensions.DependencyInjection
  测试: xUnit + Moq
  
性能目标:
  单张处理时间: < 500ms
  准确率: > 99.5%
  连续运行: > 24小时
  内存占用: < 2GB
  CPU占用: < 50%
```

### 1.3 项目结构

```
CSG-RbVision/
├── src/
│   ├── CSG.RbVision.Core/              # 核心库
│   ├── CSG.RbVision.ImageProcessing/   # 图像处理
│   ├── CSG.RbVision.Algorithm/         # 算法模块
│   ├── CSG.RbVision.Hardware/          # 硬件接口
│   ├── CSG.RbVision.Communication/     # 通信模块
│   ├── CSG.RbVision.Data/              # 数据访问
│   ├── CSG.RbVision.Business/          # 业务逻辑
│   ├── CSG.RbVision.UI.WPF/           # WPF界面
│   ├── CSG.RbVision.Configuration/     # 配置管理
│   └── CSG.RbVision.Logging/          # 日志模块
├── tests/
│   ├── CSG.RbVision.UnitTests/
│   ├── CSG.RbVision.IntegrationTests/
│   └── CSG.RbVision.PerformanceTests/
├── docs/                               # 文档
├── tools/                              # 工具脚本
├── ThirdParty/                        # 第三方SDK
└── TestData/                          # 测试数据
    ├── Images/
    └── Recipes/
```

---

## 二、技术架构

### 2.1 系统分层架构图

```
┌─────────────────────────────────────────────────────────┐
│              表示层 (Presentation Layer)                  │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │ 主控界面  │  │ 配置界面  │  │ 统计界面  │              │
│  └──────────┘  └──────────┘  └──────────┘              │
└────────────────────────┬────────────────────────────────┘
                         │ MVVM
┌────────────────────────▼────────────────────────────────┐
│            业务逻辑层 (Business Logic Layer)              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │任务管理器 │  │工作流引擎 │  │配方管理器 │              │
│  └──────────┘  └──────────┘  └──────────┘              │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│              算法层 (Algorithm Layer)                     │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │ 定位算法  │  │ 测量算法  │  │ 缺陷检测  │              │
│  └──────────┘  └──────────┘  └──────────┘              │
└────────────────────────┬────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────┐
│            硬件抽象层 (Hardware Abstraction)              │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │相机接口   │  │光源控制   │  │ IO通信   │              │
│  └──────────┘  └──────────┘  └──────────┘              │
└─────────────────────────────────────────────────────────┘
```

### 2.2 核心执行流程

```
系统启动 → 初始化硬件 → 加载配方 → 进入主循环
                                          │
                 ┌────────────────────────┘
                 │
                 ▼
         等待触发信号 ◄──────────┐
                 │               │
                 ▼               │
            采集图像              │
                 │               │
                 ▼               │
           图像预处理             │
                 │               │
                 ▼               │
          执行算法流程            │
                 │               │
                 ▼               │
            结果判定              │
                 │               │
                 ▼               │
     输出结果(IO/通信/保存)        │
                 │               │
                 └───────────────┘
```

---

## 三、开发环境准备

### 3.1 必装软件清单

```powershell
# 1. Visual Studio 2022
# 下载地址: https://visualstudio.microsoft.com/
# 必选工作负载:
#   - .NET 桌面开发
#   - 通用 Windows 平台开发（可选）

# 2. .NET 8 SDK
# 验证安装: 
dotnet --version  # 应显示 8.0.x

# 3. Git
# 验证:
git --version

# 4. Cursor (AI编程助手)
# 下载地址: https://cursor.sh/

# 5. 推荐安装的VS扩展
# - ReSharper (或使用内置分析器)
# - CodeMaid (代码清理)
# - Productivity Power Tools
```

### 3.2 项目初始化脚本

创建 `tools/init-project.ps1`:

```powershell
# 初始化项目结构
$projectRoot = "D:\VS项目\CSG-RbVision"
$folders = @(
    "src\CSG.RbVision.Core",
    "src\CSG.RbVision.ImageProcessing",
    "src\CSG.RbVision.Algorithm",
    "src\CSG.RbVision.Hardware",
    "src\CSG.RbVision.Communication",
    "src\CSG.RbVision.Data",
    "src\CSG.RbVision.Business",
    "src\CSG.RbVision.UI.WPF",
    "src\CSG.RbVision.Configuration",
    "src\CSG.RbVision.Logging",
    "tests\CSG.RbVision.UnitTests",
    "tests\CSG.RbVision.IntegrationTests",
    "tests\CSG.RbVision.PerformanceTests",
    "docs",
    "tools",
    "ThirdParty",
    "TestData\Images\OK",
    "TestData\Images\NG",
    "TestData\Recipes"
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Force -Path "$projectRoot\$folder"
}

# 创建解决方案
cd $projectRoot
dotnet new sln -n CSG-RbVision

Write-Host "✅ 项目结构创建完成" -ForegroundColor Green
```

---

## ⚠️ CURSOR AI 使用关键注意事项

**在开始任何编码前，请仔细阅读本节！**

### Cursor使用的核心原则

1. **分模块逐步实现，不要一次性生成整个项目**
2. **每个接口先定义再实现**
3. **先写测试用例，再写实现代码（TDD）**
4. **内存管理是重中之重，每个涉及非托管资源的类必须实现IDisposable**

### 关键代码模式（告诉Cursor遵循这些）

#### ❌ 禁止模式

```csharp
// 1. 禁止：忘记释放Mat
var image = Cv2.ImRead("test.jpg");
Cv2.GaussianBlur(image, gray, new Size(5, 5), 0);
// 内存泄漏！

// 2. 禁止：在相机回调中做耗时操作
camera.ImageGrabbed += (s, e) => {
    var result = ProcessImage(e.Image); // 阻塞相机采集！
};

// 3. 禁止：在UI线程做图像处理
private void Button_Click(object sender, RoutedEventArgs e) {
    var result = HeavyImageProcessing(); // UI卡死！
}
```

#### ✅ 必须遵循的模式

```csharp
// 1. 必须：所有Mat使用using
using (var image = Cv2.ImRead("test.jpg"))
using (var gray = new Mat())
{
    Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
}

// 2. 必须：相机回调快速返回
camera.ImageGrabbed += (s, e) => {
    var imageCopy = e.Image.Clone(); // 立即复制
    _imageChannel.Writer.TryWrite(imageCopy); // 非阻塞入队
};

// 3. 必须：UI操作在UI线程，处理在后台线程
private async void Button_Click(object sender, RoutedEventArgs e) {
    var result = await Task.Run(() => HeavyImageProcessing());
    await Dispatcher.InvokeAsync(() => DisplayResult(result));
}
```

### 告诉Cursor的核心指令模板

```
当生成涉及OpenCV的代码时，你必须：
1. 所有Mat对象必须在using块中或明确调用Dispose()
2. 避免在循环中创建临时Mat，使用对象池或预分配
3. 对于大图像（>1MB），使用Span<T>和指针操作

当生成相机采集代码时，你必须：
1. 相机回调中只做数据复制，立即返回
2. 使用Channel<T>或ConcurrentQueue<T>异步处理
3. 必须处理相机断线、超时等异常

当生成WPF UI代码时，你必须：
1. ViewModel不能直接依赖View
2. 图像更新使用WriteableBitmap，在后台线程转换
3. 使用Dispatcher.InvokeAsync而非Invoke避免死锁
```

---

