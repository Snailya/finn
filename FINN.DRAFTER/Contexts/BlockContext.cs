using Microsoft.EntityFrameworkCore;

namespace FINN.DRAFTER.Contexts;

public class BlockContext : DbContext
{
    public BlockContext(DbContextOptions<BlockContext> options) : base(options)
    {
    }

    public DbSet<BlockDefinition> BlockDefinitions { get; set; }
}