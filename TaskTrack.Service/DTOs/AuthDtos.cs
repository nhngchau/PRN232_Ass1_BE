using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class RegisterRequestDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AccountDto(int AccountId, string FullName, string Email, int Role);

public record LoginResponseDto(string Token, DateTime ExpiresAt, AccountDto Account);

public class AuthServiceResult<T>
{
    public bool Succeeded { get; private init; }
    public T? Data { get; private init; }
    public string? Error { get; private init; }
    public bool Conflict { get; private init; }
    public bool Unauthorized { get; private init; }

    public static AuthServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static AuthServiceResult<T> Duplicate(string error) => new() { Error = error, Conflict = true };
    public static AuthServiceResult<T> InvalidCredentials(string error) => new() { Error = error, Unauthorized = true };
}
