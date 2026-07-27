using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class ProjectLanguageConfiguration : IEntityTypeConfiguration<ProjectLanguage>
{
    public void Configure(EntityTypeBuilder<ProjectLanguage> builder)
    {
        builder.ToTable("ProjectLanguages");
        builder.HasKey(x => new
        {
            x.ProjectId,
            x.LanguageId
        });
        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectLanguages)
            .HasForeignKey(x => x.ProjectId);
        builder.HasOne(x => x.Language)
            .WithMany(x => x.ProjectLanguages)
            .HasForeignKey(x => x.LanguageId);
    }
}