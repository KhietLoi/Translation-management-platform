using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration: IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");
        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("now()");
        
        
    }
}