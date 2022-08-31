using FINN.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FINN.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RequestLog> Logs { get; set; }
}