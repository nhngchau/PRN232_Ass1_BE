using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class AuthService : IAuthService
{
    private const int StaffRole = 0;
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IAccountRepository accountRepository, IPasswordHasher passwordHasher)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
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

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static AccountDto ToDto(SystemAccount account) =>
        new(account.AccountId, account.FullName, account.Email, account.Role);
}
