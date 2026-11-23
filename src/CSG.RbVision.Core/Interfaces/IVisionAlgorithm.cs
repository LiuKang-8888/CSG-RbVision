using CSG.RbVision.Core.Models;

namespace CSG.RbVision.Core.Interfaces;

/// <summary>
/// 视觉算法接口 - 所有算法的统一契约
/// </summary>
public interface IVisionAlgorithm : IDisposable
{
    /// <summary>算法名称</summary>
    string Name { get; }

    /// <summary>算法类型</summary>
    AlgorithmType Type { get; }

    /// <summary>
    /// 执行算法
    /// </summary>
    /// <param name="context">工作流上下文</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<AlgorithmResult> ExecuteAsync(WorkflowContext context, CancellationToken cancellationToken = default);

    /// <summary>释放算法资源（模型、缓存等）</summary>
    void ReleaseResources();
}
