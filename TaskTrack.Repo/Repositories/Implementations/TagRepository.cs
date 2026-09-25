using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;

namespace TaskTrack.Repo.Repositories.Implementations;

public class TagRepository : ITagRepository
{
    private readonly TaskManagementDbContext _context;

    public TagRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public Task<List<Tag>> GetAllAsync() =>
        _context.Tags.AsNoTracking().OrderBy(t => t.TagName).ToListAsync();

    public Task<Tag?> GetByIdAsync(int id) =>
        _context.Tags.FirstOrDefaultAsync(t => t.TagId == id);

    public Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids) =>
        _context.Tags.Where(t => ids.Contains(t.TagId)).ToListAsync();

    public global::System.Threading.Tasks.Task<bool> IsUsedAsync(int id) =>
        _context.Tasks.AnyAsync(t => t.Tags.Any(tag => tag.TagId == id));

    public global::System.Threading.Tasks.Task<bool> NameExistsAsync(string name, int? excludingId = null)
    {
        var normalized = name.Trim().ToLower();
        return _context.Tags.AnyAsync(t =>
            t.TagName.ToLower() == normalized &&
            (!excludingId.HasValue || t.TagId != excludingId.Value));
    }

    public global::System.Threading.Tasks.Task AddAsync(Tag tag) => _context.Tags.AddAsync(tag).AsTask();

    public void Update(Tag tag) => _context.Tags.Update(tag);

    public void Delete(Tag tag) => _context.Tags.Remove(tag);

    public global::System.Threading.Tasks.Task SaveChangesAsync() => _context.SaveChangesAsync();
}
