using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetActiveAsync();
    Task<Project?> GetDetailAsync(int id);
    Task<Project?> GetByIdAsync(int id);
    Task<List<Project>> GetByDepartmentAsync(int departmentId);
    Task<List<Project>> SearchAsync(string? name, short? status, int? departmentId);
    global::System.Threading.Tasks.Task<bool> HasTasksAsync(int id);
    global::System.Threading.Tasks.Task AddAsync(Project project);
    void Update(Project project);
    void Delete(Project project);
    global::System.Threading.Tasks.Task SaveChangesAsync();
}
