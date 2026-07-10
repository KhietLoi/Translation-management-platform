using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the Role entity in the database context.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");
        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("now()");
        // Index for Role:
        builder.HasIndex(x => x.Name)
            .IsUnique();
        
    }
}