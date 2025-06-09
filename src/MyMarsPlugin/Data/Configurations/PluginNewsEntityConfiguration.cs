using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyMarsPlugin.Data.Entities;

namespace MyMarsPlugin.Data.Configurations;

public class PluginNewsEntityConfiguration : IEntityTypeConfiguration<PluginNewsEntity>
{
    public void Configure(EntityTypeBuilder<PluginNewsEntity> entity)
    {
        entity.ToTable("news");

        entity.Property(e => e.CreatedAt)
           .HasDefaultValueSql("now()");

        entity.Property(x => x.Title).HasColumnType("text");
        entity.Property(x => x.Content).HasColumnType("text");

        // Relations

        //entity.HasMany(x => x.Files)
        //    .WithMany(x => x.Posts)
        //    .UsingEntity<PluginNewsFilesEntity>(
        //        l => l.HasOne(x => x.FileEntity).WithMany(x => x.PostFiles),
        //        r => r.HasOne(x => x.Post).WithMany(x => x.PostFiles),
        //        k => k.HasKey(x => new { x.PostId, x.FileEntityId })
        //    );

    }
}
