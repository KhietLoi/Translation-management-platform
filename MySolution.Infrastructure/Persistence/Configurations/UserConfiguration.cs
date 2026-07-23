using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

/// <summary>
///     Represents the configuration for the User entity in the database context.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.PasswordHash)
            .IsRequired();
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
        builder.Property(x => x.IsEmailVerified)
            .HasDefaultValue(false);
        builder.Property(x => x.SecurityStamp)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");
        builder.HasIndex(x => x.Email)
            .IsUnique();
        builder.HasIndex(x => x.Username)
            .IsUnique();
    }
}