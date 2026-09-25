using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[Route("api/tasks")]
public class TasksController : ApiControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _taskService.GetActiveAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => FromResult(await _taskService.GetDetailAsync(id));

    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetByProject(int projectId) => Ok(await _taskService.GetByProjectAsync(projectId));

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var result = await _taskService.CreateAsync(dto);
        if (!result.Succeeded)
        {
            return FromResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.TaskId }, result.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTaskDto dto) => FromResult(await _taskService.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => NoContentFromResult(await _taskService.SoftDeleteAsync(id));

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? title,
        [FromQuery] short? status,
        [FromQuery] short? priority,
        [FromQuery] int? projectId,
        [FromQuery] int? tagId) =>
        Ok(await _taskService.SearchAsync(title, status, priority, projectId, tagId));
}
