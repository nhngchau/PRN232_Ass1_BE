using WorkTask = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<List<WorkTask>> GetActiveAsync();
    Task<WorkTask?> GetDetailAsync(int id);
    Task<WorkTask?> GetByIdWithTagsAsync(int id);
    Task<List<WorkTask>> GetByProjectAsync(int projectId);
    Task<List<WorkTask>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId);
    Task AddAsync(WorkTask task);
    void Update(WorkTask task);
    Task<List<int>> GetExistingTagIdsAsync(IEnumerable<int> tagIds);
    Task SaveChangesAsync();
}
