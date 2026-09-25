using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;

namespace TaskTrack.Repo.Repositories.Implementations;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly TaskManagementDbContext _context;

    public DepartmentRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public Task<List<Department>> GetActiveAsync() =>
        _context.Departments.AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();

    public Task<Department?> GetDetailAsync(int id) =>
        _context.Departments.AsNoTracking()
            .Include(d => d.Projects.Where(p => p.IsActive))
            .FirstOrDefaultAsync(d => d.DepartmentId == id && d.IsActive);

    public Task<Department?> GetByIdAsync(int id) =>
        _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id);

    public Task<List<Department>> SearchAsync(string? name)
    {
        var query = _context.Departments.AsNoTracking().Where(d => d.IsActive);
        if (!string.IsNullOrWhiteSpace(name))
        {
            var term = name.Trim().ToLower();
            query = query.Where(d => d.DepartmentName.ToLower().Contains(term));
        }

        return query.OrderBy(d => d.DepartmentName).ToListAsync();
    }

    public global::System.Threading.Tasks.Task<bool> HasProjectsAsync(int id) =>
        _context.Projects.AnyAsync(p => p.DepartmentId == id);

    public global::System.Threading.Tasks.Task AddAsync(Department department) => _context.Departments.AddAsync(department).AsTask();

    public void Update(Department department) => _context.Departments.Update(department);

    public void Delete(Department department) => _context.Departments.Remove(department);

    public global::System.Threading.Tasks.Task SaveChangesAsync() => _context.SaveChangesAsync();
}
