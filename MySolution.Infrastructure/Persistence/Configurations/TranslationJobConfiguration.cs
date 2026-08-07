using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class TranslationJobConfiguration : IEntityTypeConfiguration<TranslationJob>
{
    public void Configure(EntityTypeBuilder<TranslationJob> builder)
    {
        builder.ToTable("TranslationJobs");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Type)
            .IsRequired();
        builder.Property(x => x.Status)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.FileName)
            .HasMaxLength(500);
        builder.Property(x => x.DownloadUrl)
            .HasMaxLength(1000); 
        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(2000);
        
        builder
            .HasOne(x => x.Project)
            .WithMany(x => x.TranslationJobs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}