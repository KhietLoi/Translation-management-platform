using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class TranslationJobConfiguration
    : IEntityTypeConfiguration<TranslationJob>
{
    public void Configure(EntityTypeBuilder<TranslationJob> builder)
    {
        builder.ToTable("TranslationJobs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        builder.Property(x => x.Type)
            .IsRequired();
        builder.Property(x => x.Status)
            .IsRequired();
        builder.Property(x => x.CreatedBy)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.TotalRecords)
            .HasDefaultValue(0);
        builder.Property(x => x.SuccessRecords)
            .HasDefaultValue(0);
        builder.Property(x => x.FailedRecords)
            .HasDefaultValue(0);
        builder.Property(x => x.SkippedRecords)
            .HasDefaultValue(0);
        builder.Property(x => x.FileType);
        builder.Property(x => x.BlobFileName)
            .HasMaxLength(500);
        builder.Property(x => x.FileName)
            .HasMaxLength(500);
        builder.Property(x => x.DownloadUrl)
            .HasMaxLength(1000);
        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);

        // Project

        builder
            .HasOne(x => x.Project)
            .WithMany(x => x.TranslationJobs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Language

        builder
            .HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder
            .HasOne(x => x.Namespace)
            .WithMany()
            .HasForeignKey(x => x.NamespaceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.LanguageId);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x =>
            new
            {
                x.ProjectId,
                x.Status
            });

        builder.HasIndex(x =>
            new
            {
                x.ProjectId,
                x.Type
            });

        builder.HasIndex(x => x.CreatedBy);
    }
}