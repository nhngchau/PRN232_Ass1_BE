using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;

namespace TaskTrack.Repo.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TaskManagementDbContext _context;

    public RefreshTokenRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public global::System.Threading.Tasks.Task<RefreshToken?> GetByHashAsync(string tokenHash) =>
        _context.RefreshTokens
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

    public global::System.Threading.Tasks.Task AddAsync(RefreshToken refreshToken) =>
        _context.RefreshTokens.AddAsync(refreshToken).AsTask();

    public void Update(RefreshToken refreshToken) => _context.RefreshTokens.Update(refreshToken);

    public global::System.Threading.Tasks.Task SaveChangesAsync() => _context.SaveChangesAsync();
}
