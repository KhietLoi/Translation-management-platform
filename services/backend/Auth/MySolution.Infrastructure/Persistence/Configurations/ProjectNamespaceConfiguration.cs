using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class ProjectNamespaceConfiguration : IEntityTypeConfiguration<ProjectNamespace>
{
    public void Configure(EntityTypeBuilder<ProjectNamespace> builder)
    {
        builder.ToTable("ProjectNamespaces");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectNamespaces)
            .HasForeignKey(x => x.ProjectId);
        builder.HasIndex(x => new
        {
            x.ProjectId,
            x.Name
        }).IsUnique();
    }
}