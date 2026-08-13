using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class TranslationReleaseConfiguration : IEntityTypeConfiguration<TranslationRelease>
{
    public void Configure(EntityTypeBuilder<TranslationRelease> builder)
    {
        builder.ToTable("TranslationReleases");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Version)
            .IsRequired();
        builder.Property(x => x.ProjectId)
            .IsRequired();
        builder.Property(x => x.BlobFileName)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(x => x.DownloadUrl)
            .HasMaxLength(1000);
        builder.Property(x => x.Checksum)
            .HasMaxLength(256);
        builder.Property(x => x.TotalKey)
            .HasDefaultValue(0);
        builder.Property(x => x.Notes)
            .HasMaxLength(1000);
        builder.Property(x => x.PublishedBy)
            .IsRequired();
        builder.Property(x => x.PublishedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");
        builder.HasOne(x => x.Project)
            .WithMany(x => x.TranslationReleases)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => new { x.ProjectId, x.Version });
        builder.HasIndex(x => new { x.ProjectId, x.IsActive });
    }
}