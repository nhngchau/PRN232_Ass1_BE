using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/profile")]
[Authorize]
public class ProfileController : ApiControllerBase
{
    private readonly IAccountService _accountService;

    public ProfileController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var accountId = GetAuthenticatedAccountId();
        return accountId.HasValue
            ? FromResult(await _accountService.GetProfileAsync(accountId.Value))
            : Unauthorized(new ApiErrorDto("Authenticated token is missing AccountID claim."));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileDto dto)
    {
        var accountId = GetAuthenticatedAccountId();
        return accountId.HasValue
            ? FromResult(await _accountService.UpdateProfileAsync(accountId.Value, dto))
            : Unauthorized(new ApiErrorDto("Authenticated token is missing AccountID claim."));
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var accountId = GetAuthenticatedAccountId();
        return accountId.HasValue
            ? NoContentFromResult(await _accountService.ChangePasswordAsync(accountId.Value, dto))
            : Unauthorized(new ApiErrorDto("Authenticated token is missing AccountID claim."));
    }

    private int? GetAuthenticatedAccountId()
    {
        var accountId = User.FindFirst("AccountID")?.Value;
        return int.TryParse(accountId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
            ? id
            : null;
    }
}
