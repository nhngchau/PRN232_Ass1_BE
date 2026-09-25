using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<List<Department>> GetActiveAsync();
    Task<Department?> GetDetailAsync(int id);
    Task<Department?> GetByIdAsync(int id);
    Task<List<Department>> SearchAsync(string? name);
    global::System.Threading.Tasks.Task<bool> HasProjectsAsync(int id);
    global::System.Threading.Tasks.Task AddAsync(Department department);
    void Update(Department department);
    void Delete(Department department);
    global::System.Threading.Tasks.Task SaveChangesAsync();
}
