using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class CreateTaskDto
{
    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, 3)]
    public short Status { get; set; }

    [Range(0, 3)]
    public short Priority { get; set; } = 1;

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    public List<int> TagIds { get; set; } = [];
}

public class UpdateTaskDto : CreateTaskDto
{
}

public record TaskDto(
    int TaskId,
    string Title,
    string? Description,
    short Status,
    short Priority,
    DateOnly? DueDate,
    int ProjectId,
    string ProjectName,
    bool IsActive,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    List<TagDto> Tags);

public record TaskDetailDto(
    int TaskId,
    string Title,
    string? Description,
    short Status,
    short Priority,
    DateOnly? DueDate,
    int ProjectId,
    string ProjectName,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedDate,
    DateTime? ModifiedDate,
    List<TagDto> Tags);
