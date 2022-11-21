using Microsoft.EntityFrameworkCore;

namespace FINN.SLIDE;

public class SlideDb : DbContext
{
    public SlideDb(DbContextOptions<SlideDb> options)
        : base(options)
    {
    }

    public DbSet<Slide> Slides => Set<Slide>();
    public DbSet<Topic> Topics => Set<Topic>();
}