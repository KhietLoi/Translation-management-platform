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
        builder.Property(x => x.DurationMs)
            .IsRequired();
        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);
        builder.Property(x => x.ApplicationId)
            .IsRequired();
        
        builder.HasOne(x => x.ApiKey)
            .WithMany()
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Application)
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}