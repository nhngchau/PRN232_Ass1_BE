namespace TaskTrack.Repo.Models;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int AccountId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public virtual SystemAccount Account { get; set; } = null!;
}
