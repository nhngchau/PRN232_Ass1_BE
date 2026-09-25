using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface ITaskService
{
    Task<List<TaskDto>> GetActiveAsync();
    Task<ServiceResult<TaskDetailDto>> GetDetailAsync(int id);
    Task<List<TaskDto>> GetByProjectAsync(int projectId);
    Task<List<TaskDto>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId);
    Task<ServiceResult<TaskDto>> CreateAsync(CreateTaskDto dto);
    Task<ServiceResult<TaskDto>> UpdateAsync(int id, UpdateTaskDto dto);
    Task<ServiceResult<bool>> SoftDeleteAsync(int id);
}
