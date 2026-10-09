using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IJwtTokenService
{
    LoginResponseDto CreateLoginResponse(AccountDto account);
}
