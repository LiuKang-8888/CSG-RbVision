namespace CSG.RbVision.Core.Models;

/// <summary>
/// 错误码枚举
/// </summary>
public enum ErrorCode
{
    /// <summary>无错误</summary>
    None = 0,

    // 相机相关 1xxx
    /// <summary>未找到相机</summary>
    CameraNotFound = 1001,

    /// <summary>相机连接失败</summary>
    CameraConnectionFailed = 1002,

    /// <summary>相机已断开</summary>
    CameraDisconnected = 1003,

    /// <summary>采图超时</summary>
    ImageGrabTimeout = 1004,

    /// <summary>采图失败</summary>
    ImageGrabFailed = 1005,

    // 算法相关 2xxx
    /// <summary>算法执行失败</summary>
    AlgorithmExecutionFailed = 2001,

    /// <summary>参数无效</summary>
    InvalidParameters = 2002,

    // 配方相关 3xxx
    /// <summary>配方未找到</summary>
    RecipeNotFound = 3001,

    /// <summary>配方加载失败</summary>
    RecipeLoadFailed = 3002,

    // 通信相关 4xxx
    /// <summary>通信失败</summary>
    CommunicationFailed = 4001,

    // 其他
    /// <summary>未知错误</summary>
    UnknownError = 9999
}
