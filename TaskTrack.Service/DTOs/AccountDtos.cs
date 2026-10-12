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

public class UpdateProfileDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}

public record ProfileDto(
    int AccountId,
    string FullName,
    string Email,
    int Role,
    DateTime CreatedDate);
