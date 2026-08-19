using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action)
            .IsRequired();
        builder.Property(x => x.EntityName)
            .HasMaxLength(100)
            .IsRequired();
    
        builder.Property(x => x.EntityId);
        builder.Property(x => x.OldValue)
            .HasColumnType("text");
        builder.Property(x => x.NewValue)
            .HasColumnType("text");
        builder.Property(x => x.Reason)
            .HasMaxLength(1000);
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Project)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new
        {
            x.EntityName,
            x.EntityId
        });
    }
}