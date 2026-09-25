using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;

namespace TaskTrack.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result.Data);
        }

        if (result.NotFound)
        {
            return NotFound(new ApiErrorDto(result.Error ?? "Resource was not found."));
        }

        if (result.ValidationErrors is not null)
        {
            return BadRequest(new ApiErrorDto(result.Error ?? "Validation failed.", result.ValidationErrors));
        }

        return BadRequest(new ApiErrorDto(result.Error ?? "Request could not be completed."));
    }

    protected IActionResult NoContentFromResult(ServiceResult<bool> result)
    {
        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.NotFound)
        {
            return NotFound(new ApiErrorDto(result.Error ?? "Resource was not found."));
        }

        return BadRequest(new ApiErrorDto(result.Error ?? "Request could not be completed."));
    }
}
