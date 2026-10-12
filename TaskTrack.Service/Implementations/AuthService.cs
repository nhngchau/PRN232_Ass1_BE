using System.Security.Cryptography;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class AuthService : IAuthService
{
    private const int StaffRole = 0;
    private readonly IAccountRepository _accountRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly int _refreshTokenDays;

    public AuthService(
        IAccountRepository accountRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher)
    {
        _accountRepository = accountRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _refreshTokenDays = GetRefreshTokenDays();
    }

    public async global::System.Threading.Tasks.Task<AuthServiceResult<AccountDto>> RegisterAsync(RegisterRequestDto dto)
    {
        var normalizedEmail = NormalizeEmail(dto.Email);
        if (await _accountRepository.EmailExistsAsync(normalizedEmail))
        {
            return AuthServiceResult<AccountDto>.Duplicate("Email already exists.");
        }

        var account = new SystemAccount
        {
            FullName = dto.FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Role = StaffRole
        };

        await _accountRepository.AddAsync(account);
        await _accountRepository.SaveChangesAsync();
        return AuthServiceResult<AccountDto>.Success(ToDto(account));
    }

    public async global::System.Threading.Tasks.Task<AuthServiceResult<AccountDto>> LoginAsync(LoginRequestDto dto)
    {
        var normalizedEmail = NormalizeEmail(dto.Email);
        var account = await _accountRepository.GetByEmailAsync(normalizedEmail);
        if (account is null || !_passwordHasher.VerifyPassword(dto.Password, account.PasswordHash))
        {
            return AuthServiceResult<AccountDto>.InvalidCredentials("Invalid email or password.");
        }

        return AuthServiceResult<AccountDto>.Success(ToDto(account));
    }

    public async global::System.Threading.Tasks.Task<AuthServiceResult<string>> CreateRefreshTokenAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
        {
            return AuthServiceResult<string>.InvalidCredentials("Account was not found.");
        }

        var refreshToken = GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            AccountId = accountId,
            TokenHash = HashRefreshToken(refreshToken),
            CreatedAt = Now(),
            ExpiresAt = Now().AddDays(_refreshTokenDays)
        });
        await _refreshTokenRepository.SaveChangesAsync();

        return AuthServiceResult<string>.Success(refreshToken);
    }

    public async global::System.Threading.Tasks.Task<AuthServiceResult<RefreshTokenResultDto>> RefreshAsync(string refreshToken)
    {
        var tokenHash = HashRefreshToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByHashAsync(tokenHash);
        if (storedToken is null || storedToken.RevokedAt.HasValue || storedToken.ExpiresAt <= Now())
        {
            return AuthServiceResult<RefreshTokenResultDto>.InvalidCredentials("Refresh token is invalid or expired.");
        }

        var replacementToken = GenerateRefreshToken();
        storedToken.RevokedAt = Now();
        storedToken.ReplacedByTokenHash = HashRefreshToken(replacementToken);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            AccountId = storedToken.AccountId,
            TokenHash = storedToken.ReplacedByTokenHash,
            CreatedAt = Now(),
            ExpiresAt = Now().AddDays(_refreshTokenDays)
        });
        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return AuthServiceResult<RefreshTokenResultDto>.Success(
            new RefreshTokenResultDto(ToDto(storedToken.Account), replacementToken));
    }

    public async global::System.Threading.Tasks.Task RevokeRefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var storedToken = await _refreshTokenRepository.GetByHashAsync(HashRefreshToken(refreshToken));
        if (storedToken is null || storedToken.RevokedAt.HasValue)
        {
            return;
        }

        storedToken.RevokedAt = Now();
        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.SaveChangesAsync();
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static AccountDto ToDto(SystemAccount account) =>
        new(account.AccountId, account.FullName, account.Email, account.Role);

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(bytes);
    }

    private static DateTime Now() => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

    private static int GetRefreshTokenDays()
    {
        var configuredValue = Environment.GetEnvironmentVariable("REFRESH_TOKEN_DAYS");
        return int.TryParse(configuredValue, out var days) && days > 0 ? days : 7;
    }
}
