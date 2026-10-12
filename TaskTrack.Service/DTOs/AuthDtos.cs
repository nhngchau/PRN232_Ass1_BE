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

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public class LogoutRequestDto
{
    public string? RefreshToken { get; set; }
}

public record AccountDto(int AccountId, string FullName, string Email, int Role);

public class LoginResponseDto
{
    public LoginResponseDto(string accessToken, DateTime accessTokenExpiry, string refreshToken, AccountDto account)
    {
        Token = accessToken;
        AccessToken = accessToken;
        ExpiresAt = accessTokenExpiry;
        AccessTokenExpiry = accessTokenExpiry;
        RefreshToken = refreshToken;
        Account = account;
    }

    public string Token { get; }
    public string AccessToken { get; }
    public DateTime ExpiresAt { get; }
    public DateTime AccessTokenExpiry { get; }
    public string RefreshToken { get; }
    public AccountDto Account { get; }
}

public record RefreshTokenResultDto(AccountDto Account, string RefreshToken);

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
