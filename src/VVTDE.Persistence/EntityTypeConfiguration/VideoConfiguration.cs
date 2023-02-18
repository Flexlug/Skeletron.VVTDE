using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VVTDE.Models;

namespace VVTDE.Persistence.EntityTypeConfiguration;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(video => video.Id);
        builder.HasIndex(video => video.Id).IsUnique();
        builder.Property(video => video.Guid);
        builder.Property(video => video.Url).HasMaxLength(40);
    }
}