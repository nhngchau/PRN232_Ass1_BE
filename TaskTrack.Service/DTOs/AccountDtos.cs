using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class UpdateAccountDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Range(0, 1)]
    public int Role { get; set; }
}

public record AccountManagementDto(
    int AccountId,
    string FullName,
    string Email,
    int Role,
    DateTime CreatedDate);
