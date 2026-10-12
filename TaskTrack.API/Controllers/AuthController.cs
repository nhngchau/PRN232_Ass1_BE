using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result.Conflict)
        {
            return Conflict(new ApiErrorDto(result.Error ?? "Email already exists."));
        }

        return Created("/api/auth/register", result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result.Unauthorized)
        {
            return Unauthorized(new ApiErrorDto(result.Error ?? "Invalid email or password."));
        }

        var refreshTokenResult = await _authService.CreateRefreshTokenAsync(result.Data!.AccountId);
        if (refreshTokenResult.Unauthorized)
        {
            return Unauthorized(new ApiErrorDto(refreshTokenResult.Error ?? "Could not create refresh token."));
        }

        return Ok(_jwtTokenService.CreateLoginResponse(result.Data!, refreshTokenResult.Data!));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshAsync(dto.RefreshToken);
        if (result.Unauthorized)
        {
            return Unauthorized(new ApiErrorDto(result.Error ?? "Refresh token is invalid or expired."));
        }

        return Ok(_jwtTokenService.CreateLoginResponse(result.Data!.Account, result.Data.RefreshToken));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequestDto dto)
    {
        await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);
        return NoContent();
    }
}
