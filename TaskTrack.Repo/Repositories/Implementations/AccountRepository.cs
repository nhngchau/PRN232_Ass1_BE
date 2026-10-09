using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;

namespace TaskTrack.Repo.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly TaskManagementDbContext _context;

    public AccountRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public global::System.Threading.Tasks.Task<List<SystemAccount>> GetAllAsync() =>
        _context.SystemAccounts.AsNoTracking()
            .OrderBy(a => a.AccountId)
            .ToListAsync();

    public global::System.Threading.Tasks.Task<SystemAccount?> GetByIdAsync(int id) =>
        _context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == id);

    public global::System.Threading.Tasks.Task<SystemAccount?> GetByEmailAsync(string normalizedEmail) =>
        _context.SystemAccounts.FirstOrDefaultAsync(a => a.Email == normalizedEmail);

    public global::System.Threading.Tasks.Task<bool> EmailExistsAsync(string normalizedEmail) =>
        _context.SystemAccounts.AnyAsync(a => a.Email == normalizedEmail);

    public global::System.Threading.Tasks.Task<bool> HasCreatedTasksAsync(int id) =>
        _context.Database
            .SqlQueryRaw<int>(
                """
                SELECT 1 AS "Value"
                FROM "Task"
                WHERE "CreatedByAccountID" = {0}
                LIMIT 1
                """,
                id)
            .AnyAsync();

    public global::System.Threading.Tasks.Task AddAsync(SystemAccount account) => _context.SystemAccounts.AddAsync(account).AsTask();

    public void Update(SystemAccount account) => _context.SystemAccounts.Update(account);

    public void Delete(SystemAccount account) => _context.SystemAccounts.Remove(account);

    public global::System.Threading.Tasks.Task SaveChangesAsync() => _context.SaveChangesAsync();
}
