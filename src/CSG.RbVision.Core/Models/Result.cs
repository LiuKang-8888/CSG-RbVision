namespace CSG.RbVision.Core.Models;

/// <summary>
/// 操作结果包装类 - 避免使用异常控制流程
/// </summary>
/// <typeparam name="T">结果值类型</typeparam>
public readonly struct Result<T>
{
    /// <summary>是否成功</summary>
    public bool IsSuccess { get; }

    /// <summary>成功时的返回值</summary>
    public T? Value { get; }

    /// <summary>错误描述</summary>
    public string? ErrorMessage { get; }

    /// <summary>错误码</summary>
    public ErrorCode ErrorCode { get; }

    private Result(bool isSuccess, T? value, ErrorCode errorCode, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>创建成功结果</summary>
    public static Result<T> Success(T value) => new(true, value, ErrorCode.None, null);

    /// <summary>创建失败结果</summary>
    public static Result<T> Failure(ErrorCode code, string message) => new(false, default, code, message);

    /// <summary>匹配模式（函数式风格）</summary>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<ErrorCode, string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(ErrorCode, ErrorMessage!);
    }
}
