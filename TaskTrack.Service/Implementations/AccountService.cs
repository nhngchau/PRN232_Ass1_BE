using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AccountService(IAccountRepository accountRepository, IPasswordHasher passwordHasher)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<AccountManagementDto>> GetAllAsync() =>
        (await _accountRepository.GetAllAsync()).Select(ToDto).ToList();

    public async Task<ServiceResult<AccountManagementDto>> GetByIdAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account is null
            ? ServiceResult<AccountManagementDto>.Missing("Account was not found.")
            : ServiceResult<AccountManagementDto>.Success(ToDto(account));
    }

    public async Task<ServiceResult<AccountManagementDto>> UpdateAsync(int id, UpdateAccountDto dto)
    {
        var validation = ValidateUpdate(dto);
        if (validation.Count > 0)
        {
            return ServiceResult<AccountManagementDto>.Invalid(validation);
        }

        var account = await _accountRepository.GetByIdAsync(id);
        if (account is null)
        {
            return ServiceResult<AccountManagementDto>.Missing("Account was not found.");
        }

        account.FullName = dto.FullName.Trim();
        account.Role = dto.Role;
        _accountRepository.Update(account);
        await _accountRepository.SaveChangesAsync();

        return ServiceResult<AccountManagementDto>.Success(ToDto(account));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account is null)
        {
            return ServiceResult<bool>.Missing("Account was not found.");
        }

        if (await _accountRepository.HasCreatedTasksAsync(id))
        {
            return ServiceResult<bool>.Failure("Account cannot be deleted because it has created one or more tasks.");
        }

        _accountRepository.Delete(account);
        await _accountRepository.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<ProfileDto>> GetProfileAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        return account is null
            ? ServiceResult<ProfileDto>.Missing("Account was not found.")
            : ServiceResult<ProfileDto>.Success(ToProfileDto(account));
    }

    public async Task<ServiceResult<ProfileDto>> UpdateProfileAsync(int accountId, UpdateProfileDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            errors[nameof(dto.FullName)] = ["Full name is required."];
        }

        if (errors.Count > 0)
        {
            return ServiceResult<ProfileDto>.Invalid(errors);
        }

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
        {
            return ServiceResult<ProfileDto>.Missing("Account was not found.");
        }

        account.FullName = dto.FullName.Trim();
        _accountRepository.Update(account);
        await _accountRepository.SaveChangesAsync();

        return ServiceResult<ProfileDto>.Success(ToProfileDto(account));
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(int accountId, ChangePasswordDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
        {
            errors[nameof(dto.CurrentPassword)] = ["Current password is required."];
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
        {
            errors[nameof(dto.NewPassword)] = ["New password must be at least 6 characters."];
        }

        if (errors.Count > 0)
        {
            return ServiceResult<bool>.Invalid(errors);
        }

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
        {
            return ServiceResult<bool>.Missing("Account was not found.");
        }

        if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, account.PasswordHash))
        {
            return ServiceResult<bool>.Invalid(new Dictionary<string, string[]>
            {
                [nameof(dto.CurrentPassword)] = ["Current password is incorrect."]
            });
        }

        account.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        _accountRepository.Update(account);
        await _accountRepository.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }

    private static Dictionary<string, string[]> ValidateUpdate(UpdateAccountDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            errors[nameof(dto.FullName)] = ["Full name is required."];
        }

        if (dto.Role is not 0 and not 1)
        {
            errors[nameof(dto.Role)] = ["Role must be 0 (Staff) or 1 (Admin)."];
        }

        return errors;
    }

    private static AccountManagementDto ToDto(SystemAccount account) =>
        new(account.AccountId, account.FullName, account.Email, account.Role, account.CreatedDate);

    private static ProfileDto ToProfileDto(SystemAccount account) =>
        new(account.AccountId, account.FullName, account.Email, account.Role, account.CreatedDate);
}
