namespace CQRS.Base;

public sealed class Result<TResponse>
{
    public bool IsSuccess { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public TResponse Value { get; }

    private Result(bool isSuccess, TResponse value, string errorCode, string errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = new Dictionary<string, string[]>().AsReadOnly();
    }
    private Result(bool isSuccess, TResponse value, string errorCode, string errorMessage, IReadOnlyDictionary<string, string[]> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = errors;
    }

    public static Result<TResponse> Success(TResponse value) => new(true, value, string.Empty, string.Empty);
    public static Result<TResponse> Failure(string errorMessage) => new(false, default!, "Failure", errorMessage);
    public static Result<TResponse> Failure(string errorCode, string errorMessage) => new(false, default!, errorCode, errorMessage);
    public static Result<TResponse> NotFound(string errorMessage) => new(false, default!, "NotFound", errorMessage);
    public static Result<TResponse> NotFound(string errorMessage, IReadOnlyDictionary<string, string[]> errors) => new(false, default!, "NotFound", errorMessage, errors);
    public static Result<TResponse> BadRequest(IReadOnlyDictionary<string, string[]> errors)
        => new(false, default!, "ValidationError", "One or more validation errors occurred.", errors);
}
