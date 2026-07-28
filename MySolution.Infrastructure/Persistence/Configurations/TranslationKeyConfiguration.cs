using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class TranslationKeyConfiguration : IEntityTypeConfiguration<TranslationKey>
{
    public void Configure(EntityTypeBuilder<TranslationKey> builder)
    {
        builder.ToTable("TranslationKeys");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Namespace)
            .WithMany()
            .HasForeignKey(x => x.NamespaceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.TranslationValues)
            .WithOne(x => x.TranslationKey)
            .HasForeignKey(x => x.TranslationKeyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new
        {
            x.ProjectId,
            x.NamespaceId,
            x.Key
        }).IsUnique();
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.NamespaceId);
    }
}