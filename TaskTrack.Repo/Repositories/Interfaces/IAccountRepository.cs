using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IAccountRepository
{
    global::System.Threading.Tasks.Task<List<SystemAccount>> GetAllAsync();
    global::System.Threading.Tasks.Task<SystemAccount?> GetByIdAsync(int id);
    global::System.Threading.Tasks.Task<SystemAccount?> GetByEmailAsync(string normalizedEmail);
    global::System.Threading.Tasks.Task<bool> EmailExistsAsync(string normalizedEmail);
    global::System.Threading.Tasks.Task<bool> HasCreatedTasksAsync(int id);
    global::System.Threading.Tasks.Task AddAsync(SystemAccount account);
    void Update(SystemAccount account);
    void Delete(SystemAccount account);
    global::System.Threading.Tasks.Task SaveChangesAsync();
}
