using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetActiveAsync();
    Task<ServiceResult<ProjectDetailDto>> GetDetailAsync(int id);
    Task<List<ProjectDto>> GetByDepartmentAsync(int departmentId);
    Task<List<ProjectDto>> SearchAsync(string? name, short? status, int? departmentId);
    Task<ServiceResult<ProjectDto>> CreateAsync(CreateProjectDto dto);
    Task<ServiceResult<ProjectDto>> UpdateAsync(int id, UpdateProjectDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
