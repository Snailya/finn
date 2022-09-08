using FINN.COST.Models;
using Microsoft.EntityFrameworkCore;

namespace FINN.COST.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Formula>()
            .HasAlternateKey(x => x.Type);
    }

    public DbSet<Formula> Formulas { get; set; }
}