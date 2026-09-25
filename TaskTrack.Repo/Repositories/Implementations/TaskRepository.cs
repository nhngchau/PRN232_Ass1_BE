using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Repositories.Interfaces;
using WorkTask = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Repo.Repositories.Implementations;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public Task<List<WorkTask>> GetActiveAsync() =>
        _context.Tasks.AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync();

    public Task<WorkTask?> GetDetailAsync(int id) =>
        _context.Tasks.AsNoTracking()
            .Include(t => t.Project)
                .ThenInclude(p => p.Department)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.IsActive);

    public Task<WorkTask?> GetByIdWithTagsAsync(int id) =>
        _context.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.TaskId == id);

    public Task<List<WorkTask>> GetByProjectAsync(int projectId) =>
        _context.Tasks.AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive && t.ProjectId == projectId)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync();

    public Task<List<WorkTask>> SearchAsync(string? title, short? status, short? priority, int? projectId, int? tagId)
    {
        var query = _context.Tasks.AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(title))
        {
            var term = title.Trim().ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(t => t.Tags.Any(tag => tag.TagId == tagId.Value));
        }

        return query.OrderBy(t => t.DueDate).ThenBy(t => t.Title).ToListAsync();
    }

    public Task AddAsync(WorkTask task) => _context.Tasks.AddAsync(task).AsTask();

    public void Update(WorkTask task) => _context.Tasks.Update(task);

    public Task<List<int>> GetExistingTagIdsAsync(IEnumerable<int> tagIds) =>
        _context.Tags.Where(t => tagIds.Contains(t.TagId)).Select(t => t.TagId).ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
