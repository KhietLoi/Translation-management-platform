using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class ApiKeyUsageLogConfiguration : IEntityTypeConfiguration<ApiKeyUsageLog>
{
    public void Configure(EntityTypeBuilder<ApiKeyUsageLog> builder)
    {
        builder.ToTable("ApiKeyUsageLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.ApiKeyId)
            .IsRequired();
        builder.Property(x => x.Endpoint)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(x => x.Method)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(x => x.IpAddress)
            .HasMaxLength(100);
      
        builder.HasOne(x => x.ApiKey)
            .WithMany()
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}