using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/projects")]
public class ProjectsController : ApiControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _projectService.GetActiveAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => FromResult(await _projectService.GetDetailAsync(id));

    [HttpGet("department/{departmentId:int}")]
    public async Task<IActionResult> GetByDepartment(int departmentId) => Ok(await _projectService.GetByDepartmentAsync(departmentId));

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        var result = await _projectService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProjectId }, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto dto) => FromResult(await _projectService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => NoContentFromResult(await _projectService.DeleteAsync(id));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] short? status, [FromQuery] int? departmentId) =>
        Ok(await _projectService.SearchAsync(name, status, departmentId));
}
