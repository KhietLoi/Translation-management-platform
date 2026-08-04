using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class ApiKeyPermissionConfiguration : IEntityTypeConfiguration<ApiKeyPermission>
{
    public void Configure(EntityTypeBuilder<ApiKeyPermission> builder)
    {
        builder.ToTable("ApiKeyPermissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Permission)
            .IsRequired();
        
        builder.HasOne(x => x.ApiKey)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}