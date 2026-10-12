using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    global::System.Threading.Tasks.Task<RefreshToken?> GetByHashAsync(string tokenHash);
    global::System.Threading.Tasks.Task AddAsync(RefreshToken refreshToken);
    void Update(RefreshToken refreshToken);
    global::System.Threading.Tasks.Task SaveChangesAsync();
}
