namespace CQRS.Base;

public sealed class Result
{
    public bool IsSuccess { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    internal Result(bool isSuccess, string errorCode, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = new Dictionary<string, string[]>();
    }
    internal Result(bool isSuccess, string errorCode, string errorMessage, IReadOnlyDictionary<string, string[]> errors)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Errors = errors;
    }

    public static Result Success() => new(true, string.Empty, string.Empty);
    public static Result Failure(string errorMessage) => new(false, "Failure", errorMessage);
    public static Result Failure(string errorCode, string errorMessage) => new(false, errorCode, errorMessage);
    public static Result NotFound(string errorMessage) => new(false, "NotFound", errorMessage);
    public static Result NotFound(string errorMessage, IReadOnlyDictionary<string, string[]> errors) => new(false, "NotFound", errorMessage, errors);
    public static Result BadRequest(IReadOnlyDictionary<string, string[]> errors)
        => new(false, "ValidationError", "One or more validation errors occurred.", errors);


    public static Result<TResponse> Success<TResponse>(TResponse value) => new(true, value, string.Empty, string.Empty);
    public static Result<TResponse> Failure<TResponse>(string errorMessage) => new(false, default!, "Failure", errorMessage);
    public static Result<TResponse> Failure<TResponse>(string errorCode, string errorMessage) => new(false, default!, errorCode, errorMessage);
    public static Result<TResponse> NotFound<TResponse>(string errorMessage) => new(false, default!, "NotFound", errorMessage);
    public static Result<TResponse> NotFound<TResponse>(string errorMessage, IReadOnlyDictionary<string, string[]> errors) => new(false, default!, "NotFound", errorMessage, errors);
    public static Result<TResponse> BadRequest<TResponse>(IReadOnlyDictionary<string, string[]> errors)
        => new(false, default!, "ValidationError", "One or more validation errors occurred.", errors);
}