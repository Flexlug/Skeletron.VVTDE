using Microsoft.EntityFrameworkCore;
using VVTDE.Domain;
using VVTDE.Persistence.EntityTypeConfiguration;

namespace VVTDE.Persistence;

public class VideoDbContext : DbContext, IVideoDbContext
{
    public DbSet<Video> Videos { get; set; }

    public VideoDbContext(DbContextOptions<VideoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new VideoConfiguration());
        base.OnModelCreating(builder);
    }
}
