namespace SmartMeal.Application.Common;

public abstract record Result<T>
{
    public sealed record Success(T Value) : Result<T>;
    public sealed record Failure(Error Error) : Result<T>;

    public bool IsSuccess => this is Success;
    public bool IsFailure => this is Failure;

    public static Result<T> Ok(T value) => new Success(value);
    public static Result<T> Fail(Error error) => new Failure(error);
    public static Result<T> Fail(string code, string message) => new Failure(Error.Create(code, message));

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure) => this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Error),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    public T GetValueOrThrow() => this switch
    {
        Success success => success.Value,
        Failure failure => throw new InvalidOperationException($"Result is failure: {failure.Error.Message}"),
        _ => throw new InvalidOperationException("Unknown result type")
    };
}
