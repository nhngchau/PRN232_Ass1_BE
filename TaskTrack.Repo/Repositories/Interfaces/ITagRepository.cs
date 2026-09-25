using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids);
    global::System.Threading.Tasks.Task<bool> IsUsedAsync(int id);
    global::System.Threading.Tasks.Task<bool> NameExistsAsync(string name, int? excludingId = null);
    global::System.Threading.Tasks.Task AddAsync(Tag tag);
    void Update(Tag tag);
    void Delete(Tag tag);
    global::System.Threading.Tasks.Task SaveChangesAsync();
}
