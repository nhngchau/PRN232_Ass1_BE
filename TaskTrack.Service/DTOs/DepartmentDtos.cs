using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class CreateDepartmentDto
{
    [Required, MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string DepartmentDescription { get; set; } = string.Empty;
}

public class UpdateDepartmentDto : CreateDepartmentDto
{
}

public record DepartmentDto(int DepartmentId, string DepartmentName, string DepartmentDescription, bool IsActive);

public record DepartmentDetailDto(int DepartmentId, string DepartmentName, string DepartmentDescription, bool IsActive, List<ProjectDto> Projects);
