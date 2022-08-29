using FINN.DXF.Models;
using Microsoft.EntityFrameworkCore;

namespace FINN.DXF.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BlockDefinition> BlockDefinitions { get; set; }
}