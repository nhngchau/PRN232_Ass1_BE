using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class CreateProjectDto
{
    [Required, MaxLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Range(0, 3)]
    public short Status { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }
}

public class UpdateProjectDto : CreateProjectDto
{
}

public record ProjectDto(
    int ProjectId,
    string ProjectName,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    short Status,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedDate);

public record ProjectDetailDto(
    int ProjectId,
    string ProjectName,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    short Status,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedDate,
    List<TaskDto> Tasks);
