namespace CSG.RbVision.Core.Models;

/// <summary>
/// 工作流执行上下文
/// TODO: Day 4 完整实现
/// </summary>
public class WorkflowContext
{
    /// <summary>当前输入图像</summary>
    public ImageData? InputImage { get; set; }

    /// <summary>流程变量字典</summary>
    public Dictionary<string, object> Variables { get; } = new();
}
