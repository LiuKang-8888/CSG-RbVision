using CSG.RbVision.Core.Models;

namespace CSG.RbVision.Core.Interfaces;

/// <summary>
/// 光源控制器接口
/// </summary>
public interface ILightController : IDisposable
{
    /// <summary>连接到光源控制器</summary>
    /// <param name="address">设备地址或连接字符串</param>
    Task<Result<bool>> ConnectAsync(string address);

    /// <summary>设置指定通道亮度</summary>
    /// <param name="channel">通道号</param>
    /// <param name="brightness">亮度值</param>
    Task<Result<bool>> SetBrightnessAsync(int channel, int brightness);

    /// <summary>设置指定通道颜色（彩色/RGB 光源）</summary>
    /// <param name="channel">通道号</param>
    /// <param name="color">目标颜色</param>
    Task<Result<bool>> SetColorAsync(int channel, Color color);
}
