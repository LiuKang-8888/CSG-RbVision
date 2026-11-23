namespace CSG.RbVision.Core.Models;

/// <summary>
/// 图像采集事件参数
/// </summary>
public class ImageCapturedEventArgs : EventArgs
{
    /// <summary>采集到的图像</summary>
    public ImageData Image { get; }

    /// <summary>事件时间戳</summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// 初始化图像采集事件参数
    /// </summary>
    /// <param name="image">采集到的图像</param>
    public ImageCapturedEventArgs(ImageData image)
    {
        Image = image;
        Timestamp = DateTime.Now;
    }
}
