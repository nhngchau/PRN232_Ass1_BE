using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IAccountService
{
    Task<List<AccountManagementDto>> GetAllAsync();
    Task<ServiceResult<AccountManagementDto>> GetByIdAsync(int id);
    Task<ServiceResult<AccountManagementDto>> UpdateAsync(int id, UpdateAccountDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<ProfileDto>> GetProfileAsync(int accountId);
    Task<ServiceResult<ProfileDto>> UpdateProfileAsync(int accountId, UpdateProfileDto dto);
    Task<ServiceResult<bool>> ChangePasswordAsync(int accountId, ChangePasswordDto dto);
}
