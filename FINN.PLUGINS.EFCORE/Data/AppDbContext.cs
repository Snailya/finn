using FINN.CORE.Models;
using Microsoft.EntityFrameworkCore;

namespace FINN.PLUGINS.EFCORE.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BlockDefinition> BlockDefinitions { get; set; }
    public DbSet<Job> Jobs { get; set; }
}