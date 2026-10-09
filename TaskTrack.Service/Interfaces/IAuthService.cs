using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult<AccountDto>> RegisterAsync(RegisterRequestDto dto);
    Task<AuthServiceResult<AccountDto>> LoginAsync(LoginRequestDto dto);
}
