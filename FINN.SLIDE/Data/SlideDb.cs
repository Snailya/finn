using FINN.SLIDE.Data;
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

    public void Seed()
    {
        Topics.AddRange(
            new Topic
            {
                Name = "1分钟",
                IsFast = true
            },
            new Topic
            {
                Name = "公司信息区",
                Topics = new List<Topic>
                {
                    new() { Name = "基本信息" },
                    new() { Name = "人力资源" },
                    new() { Name = "财务状况" },
                    new() { Name = "荣誉状况" }
                }
            },
            new Topic
            {
                Name = "涂装院信息区",
                Topics = new List<Topic>
                {
                    new() { Name = "基本信息" },
                    new() { Name = "人力资源" },
                    new() { Name = "财务状况" },
                    new() { Name = "客户信息" }
                }
            },
            new Topic { Name = "制造能力区" },
            new Topic { Name = "项目管理能力区" },
            new Topic { Name = "业绩文件区" },
            new Topic { Name = "专题核心技术区" },
            new Topic { Name = "专题对比区" });

        SaveChanges();
    }
}