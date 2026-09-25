namespace TaskTrack.Service.DTOs;

public record ApiErrorDto(string Message, Dictionary<string, string[]>? Errors = null);

public class ServiceResult<T>
{
    public bool Succeeded { get; private init; }
    public T? Data { get; private init; }
    public string? Error { get; private init; }
    public Dictionary<string, string[]>? ValidationErrors { get; private init; }
    public bool NotFound { get; private init; }

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static ServiceResult<T> Failure(string error) => new() { Error = error };
    public static ServiceResult<T> Invalid(Dictionary<string, string[]> errors) => new() { Error = "Validation failed.", ValidationErrors = errors };
    public static ServiceResult<T> Missing(string error = "Resource was not found.") => new() { Error = error, NotFound = true };
}
