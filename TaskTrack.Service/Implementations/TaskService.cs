using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;
using WorkTask = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITagRepository _tagRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, ITagRepository tagRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _tagRepository = tagRepository;
    }

    public async Task<List<TaskDto>> GetActiveAsync() =>
        (await _taskRepository.GetActiveAsync()).Select(t => t.ToDto()).ToList();

    public async Task<ServiceResult<TaskDetailDto>> GetDetailAsync(int id)
    {
        var task = await _taskRepository.GetDetailAsync(id);
        return task is null
            ? ServiceResult<TaskDetailDto>.Missing("Task was not found.")
            : ServiceResult<TaskDetailDto>.Success(task.ToDetailDto());
    }

    public async Task<List<TaskDto>> GetByProjectAsync(int projectId) =>
        (await _taskRepository.GetByProjectAsync(projectId)).Select(t => t.ToDto()).ToList();

    public async Task<List<TaskDto>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId) =>
        (await _taskRepository.SearchAsync(title, status, priority, projectId, tagId)).Select(t => t.ToDto()).ToList();

    public async Task<ServiceResult<TaskDto>> CreateAsync(CreateTaskDto dto)
    {
        var validation = await ValidateTaskAsync(dto);
        if (validation.Count > 0)
        {
            return ServiceResult<TaskDto>.Invalid(validation);
        }

        var tags = await _tagRepository.GetByIdsAsync(dto.TagIds.Distinct());
        var task = new WorkTask
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            IsActive = true
        };

        foreach (var tag in tags)
        {
            task.Tags.Add(tag);
        }

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();
        var detail = await _taskRepository.GetDetailAsync(task.TaskId);
        return ServiceResult<TaskDto>.Success(detail!.ToDto());
    }

    public async Task<ServiceResult<TaskDto>> UpdateAsync(int id, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdWithTagsAsync(id);
        if (task is null || !task.IsActive)
        {
            return ServiceResult<TaskDto>.Missing("Task was not found.");
        }

        var validation = await ValidateTaskAsync(dto);
        if (validation.Count > 0)
        {
            return ServiceResult<TaskDto>.Invalid(validation);
        }

        var tags = await _tagRepository.GetByIdsAsync(dto.TagIds.Distinct());
        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim();
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.ProjectId = dto.ProjectId;
        task.ModifiedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        task.Tags.Clear();
        foreach (var tag in tags)
        {
            task.Tags.Add(tag);
        }

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
        var detail = await _taskRepository.GetDetailAsync(id);
        return ServiceResult<TaskDto>.Success(detail!.ToDto());
    }

    public async Task<ServiceResult<bool>> SoftDeleteAsync(int id)
    {
        var task = await _taskRepository.GetByIdWithTagsAsync(id);
        if (task is null || !task.IsActive)
        {
            return ServiceResult<bool>.Missing("Task was not found.");
        }

        task.IsActive = false;
        task.ModifiedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }

    private async Task<Dictionary<string, string[]>> ValidateTaskAsync(CreateTaskDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        var project = await _projectRepository.GetByIdAsync(dto.ProjectId);
        if (project is null || !project.IsActive)
        {
            errors[nameof(dto.ProjectId)] = ["Active project was not found."];
        }

        var distinctTagIds = dto.TagIds.Distinct().ToList();
        if (distinctTagIds.Count > 0)
        {
            var existingIds = await _taskRepository.GetExistingTagIdsAsync(distinctTagIds);
            var missing = distinctTagIds.Except(existingIds).ToList();
            if (missing.Count > 0)
            {
                errors[nameof(dto.TagIds)] = [$"Tag ids were not found: {string.Join(", ", missing)}."];
            }
        }

        return errors;
    }
}
