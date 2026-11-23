# CSG-RbVision

工业视觉检测平台

- 解决方案: `CSG-RbVision.sln`（名称不变）
- 类库 TFM: `net8.0`；UI/Tests: `net8.0-windows`
- 平台: `x64`
- UI 主题包: ModernWpfUI（MahApps.Metro 3.0.0 不可用时的替代）

## 项目

1. CSG.RbVision.Core - 核心接口/注册中心/运行时
2. CSG.RbVision.Shared - 共享模型/工具/扩展
3. CSG.RbVision.Hardware - 相机/光源/IO
4. CSG.RbVision.ImageProcessing - OpenCV 封装
5. CSG.RbVision.Algorithm - 视觉算法
6. CSG.RbVision.Business - 业务/工作流
7. CSG.RbVision.Communication - MES/Modbus/串口/MQTT
8. CSG.RbVision.Data - EF Core 数据访问
9. CSG.RbVision.UI.WPF - WPF 界面
10. CSG.RbVision.Tests - 单元测试
