using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class TranslationValueConfiguration : IEntityTypeConfiguration<TranslationValue>
{
    public void Configure(EntityTypeBuilder<TranslationValue> builder)
    {
        builder.ToTable("TranslationValues");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Value).HasColumnType("text");
        builder.Property(x => x.Status)
            .HasConversion<int>().HasDefaultValue(TranslationStatus.Draft);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.TranslatedAt);
        builder.Property(x => x.ReviewedAt);
        builder.Property(x => x.PublishedAt);
        builder.HasOne(x => x.TranslationKey)
            .WithMany(x => x.TranslationValues)
            .HasForeignKey(x => x.TranslationKeyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Translator)
            .WithMany()
            .HasForeignKey(x => x.TranslatedBy)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Reviewer)
            .WithMany()
            .HasForeignKey(x => x.ReviewedBy)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Publisher)
            .WithMany()
            .HasForeignKey(x => x.PublishedBy)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => new
        {
            x.TranslationKeyId,
            x.LanguageId
        }).IsUnique();
        builder.HasIndex(x => x.LanguageId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.TranslatedBy);
        builder.HasIndex(x => x.ReviewedBy);
        builder.HasIndex(x => x.PublishedBy);
    }
}