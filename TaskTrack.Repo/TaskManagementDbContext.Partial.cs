using Microsoft.EntityFrameworkCore;

namespace TaskTrack.Repo;

public partial class TaskManagementDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Task>()
            .Property(e => e.Priority)
            .ValueGeneratedNever();
    }
}
