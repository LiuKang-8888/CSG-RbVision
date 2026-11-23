namespace CSG.RbVision.Core.Models;

/// <summary>
/// 算法执行结果基类
/// TODO: Day 4 完整实现
/// </summary>
public class AlgorithmResult
{
    /// <summary>是否成功（业务判定）</summary>
    public bool Success { get; set; }

    /// <summary>错误或说明信息</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>执行耗时（毫秒）</summary>
    public long ExecutionTimeMs { get; set; }
}
