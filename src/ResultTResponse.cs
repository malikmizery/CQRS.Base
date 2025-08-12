namespace CQRS.Base;

public sealed class Result<TResponse>
{
    public bool IsSuccess { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public TResponse Value { get; }

    internal Result(bool isSuccess, TResponse value)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = string.Empty;
        ErrorMessage = string.Empty;
        Errors = new Dictionary<string, string[]>();
    }
    internal Result(bool isSuccess, TResponse value, string errorCode, string errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = new Dictionary<string, string[]>();
    }
    internal Result(bool isSuccess, TResponse value, string errorCode, string errorMessage, IReadOnlyDictionary<string, string[]> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = errors;
    }


    public static implicit operator Result<TResponse>(TResponse value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<TResponse>(true, value);
    }
}