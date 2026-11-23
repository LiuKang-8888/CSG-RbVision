using CSG.RbVision.Core.Models;

namespace CSG.RbVision.Core.Interfaces;

/// <summary>
/// IO控制器接口
/// </summary>
public interface IIOController : IDisposable
{
    /// <summary>连接到 IO 设备</summary>
    /// <param name="address">设备地址或连接字符串</param>
    Task<Result<bool>> ConnectAsync(string address);

    /// <summary>写入数字输出点</summary>
    /// <param name="pin">输出点位</param>
    /// <param name="value">true 为高电平/导通</param>
    Task<Result<bool>> WriteDigitalOutputAsync(int pin, bool value);

    /// <summary>读取数字输入点</summary>
    /// <param name="pin">输入点位</param>
    /// <returns>成功时 Value 为电平状态</returns>
    Task<Result<bool>> ReadDigitalInputAsync(int pin);
}
