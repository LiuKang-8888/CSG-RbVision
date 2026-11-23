namespace CSG.RbVision.Core.Models;

/// <summary>
/// 图像数据封装类 - 管理图像内存的生命周期
/// TODO: Day 3 完整实现
/// </summary>
public sealed class ImageData : IDisposable
{
    /// <summary>图像宽度（像素）</summary>
    public int Width { get; init; }

    /// <summary>图像高度（像素）</summary>
    public int Height { get; init; }

    /// <summary>采集或创建时间戳</summary>
    public DateTime Timestamp { get; init; }

    // TODO: Day 3 添加内存管理

    /// <inheritdoc />
    public void Dispose() { }
}
