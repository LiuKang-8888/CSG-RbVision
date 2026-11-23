using CSG.RbVision.Core.Models;

namespace CSG.RbVision.Core.Interfaces;

/// <summary>
/// 相机接口 - 所有相机适配器的统一契约
/// </summary>
public interface ICamera : IDisposable
{
    /// <summary>相机唯一标识（序列号或IP）</summary>
    string DeviceId { get; }

    /// <summary>相机型号名称</summary>
    string ModelName { get; }

    /// <summary>是否已连接</summary>
    bool IsConnected { get; }

    /// <summary>打开相机连接</summary>
    Task<Result<bool>> OpenAsync(CancellationToken cancellationToken = default);

    /// <summary>关闭相机连接</summary>
    Task<Result<bool>> CloseAsync(CancellationToken cancellationToken = default);

    /// <summary>采集单张图像（同步触发模式）</summary>
    /// <param name="timeoutMs">超时时间（毫秒）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<Result<ImageData>> GrabImageAsync(int timeoutMs = 5000, CancellationToken cancellationToken = default);

    /// <summary>开始连续采集（异步回调模式）</summary>
    Task<Result<bool>> StartContinuousGrabAsync(CancellationToken cancellationToken = default);

    /// <summary>停止连续采集</summary>
    Task<Result<bool>> StopContinuousGrabAsync(CancellationToken cancellationToken = default);

    /// <summary>执行软件触发（硬件触发模式下）</summary>
    Task<Result<bool>> ExecuteSoftwareTriggerAsync();

    /// <summary>图像采集完成事件（连续采集模式）</summary>
    event EventHandler<ImageCapturedEventArgs>? ImageCaptured;

    /// <summary>相机断开连接事件</summary>
    event EventHandler? Disconnected;

    /// <summary>设置曝光时间（微秒）</summary>
    Task<Result<bool>> SetExposureTimeAsync(double microseconds);

    /// <summary>设置增益</summary>
    Task<Result<bool>> SetGainAsync(double gain);

    /// <summary>设置触发模式</summary>
    Task<Result<bool>> SetTriggerModeAsync(TriggerMode mode);
}
