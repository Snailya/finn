using Microsoft.EntityFrameworkCore;

namespace FINN.API.Contexts;

public class JobContext : DbContext
{
    public JobContext(DbContextOptions<JobContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
}