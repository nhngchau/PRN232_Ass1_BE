using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;

namespace TaskTrack.Repo.Repositories.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskManagementDbContext _context;

    public ProjectRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public Task<List<Project>> GetActiveAsync() =>
        _context.Projects.AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.IsActive)
            .OrderBy(p => p.ProjectName)
            .ToListAsync();

    public Task<Project?> GetDetailAsync(int id) =>
        _context.Projects.AsNoTracking()
            .Include(p => p.Department)
            .Include(p => p.Tasks.Where(t => t.IsActive))
                .ThenInclude(t => t.Tags)
            .FirstOrDefaultAsync(p => p.ProjectId == id && p.IsActive);

    public Task<Project?> GetByIdAsync(int id) =>
        _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id);

    public Task<List<Project>> GetByDepartmentAsync(int departmentId) =>
        _context.Projects.AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.IsActive && p.DepartmentId == departmentId)
            .OrderBy(p => p.ProjectName)
            .ToListAsync();

    public Task<List<Project>> SearchAsync(string? name, short? status, int? departmentId)
    {
        var query = _context.Projects.AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var term = name.Trim().ToLower();
            query = query.Where(p => p.ProjectName.ToLower().Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (departmentId.HasValue)
        {
            query = query.Where(p => p.DepartmentId == departmentId.Value);
        }

        return query.OrderBy(p => p.ProjectName).ToListAsync();
    }

    public global::System.Threading.Tasks.Task<bool> HasTasksAsync(int id) =>
        _context.Tasks.AnyAsync(t => t.ProjectId == id);

    public global::System.Threading.Tasks.Task AddAsync(Project project) => _context.Projects.AddAsync(project).AsTask();

    public void Update(Project project) => _context.Projects.Update(project);

    public void Delete(Project project) => _context.Projects.Remove(project);

    public global::System.Threading.Tasks.Task SaveChangesAsync() => _context.SaveChangesAsync();
}
