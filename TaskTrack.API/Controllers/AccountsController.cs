using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/accounts")]
[Authorize(Roles = "Admin")]
public class AccountsController : ApiControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _accountService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => FromResult(await _accountService.GetByIdAsync(id));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAccountDto dto) =>
        FromResult(await _accountService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => NoContentFromResult(await _accountService.DeleteAsync(id));
}
