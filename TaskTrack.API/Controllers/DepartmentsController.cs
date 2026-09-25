using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/departments")]
public class DepartmentsController : ApiControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _departmentService.GetActiveAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => FromResult(await _departmentService.GetDetailAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentDto dto)
    {
        var result = await _departmentService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.DepartmentId }, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto) => FromResult(await _departmentService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => NoContentFromResult(await _departmentService.DeleteAsync(id));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name) => Ok(await _departmentService.SearchAsync(name));
}
