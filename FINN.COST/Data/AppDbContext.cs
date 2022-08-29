using FINN.COST.Models;
using Microsoft.EntityFrameworkCore;

namespace FINN.COST.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cost> Costs { get; set; }
}