namespace CSG.RbVision.Core.Models;

/// <summary>
/// 算法类型
/// </summary>
public enum AlgorithmType
{
    /// <summary>图像输入</summary>
    ImageInput,

    /// <summary>预处理</summary>
    Preprocessing,

    /// <summary>定位</summary>
    Localization,

    /// <summary>测量</summary>
    Measurement,

    /// <summary>检测</summary>
    Inspection,

    /// <summary>识别</summary>
    Recognition,

    /// <summary>深度学习</summary>
    DeepLearning,

    /// <summary>逻辑</summary>
    Logic,

    /// <summary>输出</summary>
    Output
}
