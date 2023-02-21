using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VVTDE.Domain;

namespace VVTDE.Persistence.EntityTypeConfiguration;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(video => video.Id);
        builder.HasIndex(video => video.Id).IsUnique();
        builder.Property(video => video.Guid);
        builder.Property(video => video.Url).HasMaxLength(40); 
        builder.Property(video => video.Title).HasMaxLength(128); // VK API limits
        builder.Property(video => video.Description).HasMaxLength(1000); // OpenGraph recommendations. VK API limits it to 5000 chars
        builder.Property(video => video.ImageUrl).HasMaxLength(1000); // Ограничение взято от балды
    }
}