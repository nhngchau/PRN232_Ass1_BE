using TaskTrack.Repo.Models;
using TaskTrack.Service.DTOs;
using WorkTask = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Service.Mapping;

public static class DtoMapper
{
    public static DepartmentDto ToDto(this Department department) =>
        new(department.DepartmentId, department.DepartmentName, department.DepartmentDescription, department.IsActive);

    public static DepartmentDetailDto ToDetailDto(this Department department) =>
        new(
            department.DepartmentId,
            department.DepartmentName,
            department.DepartmentDescription,
            department.IsActive,
            department.Projects.Select(p => p.ToDto()).ToList());

    public static ProjectDto ToDto(this Project project) =>
        new(
            project.ProjectId,
            project.ProjectName,
            project.Description,
            project.StartDate,
            project.EndDate,
            project.Status,
            project.DepartmentId,
            project.Department.DepartmentName,
            project.IsActive,
            project.CreatedDate);

    public static ProjectDetailDto ToDetailDto(this Project project) =>
        new(
            project.ProjectId,
            project.ProjectName,
            project.Description,
            project.StartDate,
            project.EndDate,
            project.Status,
            project.DepartmentId,
            project.Department.DepartmentName,
            project.IsActive,
            project.CreatedDate,
            project.Tasks.Select(t => t.ToDto()).ToList());

    public static TaskDto ToDto(this WorkTask task) =>
        new(
            task.TaskId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.DueDate,
            task.ProjectId,
            task.Project.ProjectName,
            task.IsActive,
            task.CreatedDate,
            task.ModifiedDate,
            task.Tags.Select(t => t.ToDto()).ToList());

    public static TaskDetailDto ToDetailDto(this WorkTask task) =>
        new(
            task.TaskId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.DueDate,
            task.ProjectId,
            task.Project.ProjectName,
            task.Project.DepartmentId,
            task.Project.Department.DepartmentName,
            task.IsActive,
            task.CreatedDate,
            task.ModifiedDate,
            task.Tags.Select(t => t.ToDto()).ToList());

    public static TagDto ToDto(this Tag tag) => new(tag.TagId, tag.TagName, tag.Color);
}
