using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class CreateTagDto
{
    [Required, MaxLength(50)]
    public string TagName { get; set; } = string.Empty;

    [RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Color must be a hex color like #3B82F6.")]
    public string? Color { get; set; }
}

public class UpdateTagDto : CreateTagDto
{
}

public record TagDto(int TagId, string TagName, string? Color);
