using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Mapping;

namespace TaskTrack.Service.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public ProjectService(IProjectRepository projectRepository, IDepartmentRepository departmentRepository)
    {
        _projectRepository = projectRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<List<ProjectDto>> GetActiveAsync() =>
        (await _projectRepository.GetActiveAsync()).Select(p => p.ToDto()).ToList();

    public async Task<ServiceResult<ProjectDetailDto>> GetDetailAsync(int id)
    {
        var project = await _projectRepository.GetDetailAsync(id);
        return project is null
            ? ServiceResult<ProjectDetailDto>.Missing("Project was not found.")
            : ServiceResult<ProjectDetailDto>.Success(project.ToDetailDto());
    }

    public async Task<List<ProjectDto>> GetByDepartmentAsync(int departmentId) =>
        (await _projectRepository.GetByDepartmentAsync(departmentId)).Select(p => p.ToDto()).ToList();

    public async Task<List<ProjectDto>> SearchAsync(string? name, short? status, int? departmentId) =>
        (await _projectRepository.SearchAsync(name, status, departmentId)).Select(p => p.ToDto()).ToList();

    public async Task<ServiceResult<ProjectDto>> CreateAsync(CreateProjectDto dto)
    {
        var validation = await ValidateProjectAsync(dto);
        if (validation.Count > 0)
        {
            return ServiceResult<ProjectDto>.Invalid(validation);
        }

        var project = new Project
        {
            ProjectName = dto.ProjectName.Trim(),
            Description = dto.Description?.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            DepartmentId = dto.DepartmentId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();
        var detail = await _projectRepository.GetDetailAsync(project.ProjectId);
        return ServiceResult<ProjectDto>.Success(detail!.ToDto());
    }

    public async Task<ServiceResult<ProjectDto>> UpdateAsync(int id, UpdateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null || !project.IsActive)
        {
            return ServiceResult<ProjectDto>.Missing("Project was not found.");
        }

        var validation = await ValidateProjectAsync(dto);
        if (validation.Count > 0)
        {
            return ServiceResult<ProjectDto>.Invalid(validation);
        }

        project.ProjectName = dto.ProjectName.Trim();
        project.Description = dto.Description?.Trim();
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.Status = dto.Status;
        project.DepartmentId = dto.DepartmentId;
        _projectRepository.Update(project);
        await _projectRepository.SaveChangesAsync();

        var detail = await _projectRepository.GetDetailAsync(id);
        return ServiceResult<ProjectDto>.Success(detail!.ToDto());
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            return ServiceResult<bool>.Missing("Project was not found.");
        }

        if (await _projectRepository.HasTasksAsync(id))
        {
            return ServiceResult<bool>.Failure("Project cannot be deleted because tasks are linked to it.");
        }

        _projectRepository.Delete(project);
        await _projectRepository.SaveChangesAsync();
        return ServiceResult<bool>.Success(true);
    }

    private async Task<Dictionary<string, string[]>> ValidateProjectAsync(CreateProjectDto dto)
    {
        var errors = new Dictionary<string, string[]>();
        if (dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate)
        {
            errors[nameof(dto.EndDate)] = ["End date cannot be earlier than start date."];
        }

        var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (department is null || !department.IsActive)
        {
            errors[nameof(dto.DepartmentId)] = ["Active department was not found."];
        }

        return errors;
    }
}
