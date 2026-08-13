using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.UserId)
            .IsRequired();
        builder.Property(x => x.ProjectId)
            .IsRequired();
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Message)
            .IsRequired();
        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();
        builder.Property(x => x.IsRead)
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(x => x.NavigationUrl)
            .HasMaxLength(500);
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.TriggeredByUser)
            .WithMany()
            .HasForeignKey(x => x.TriggeredByUserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new
        {
            x.UserId,
            x.ProjectId
        });
        builder.HasIndex(x => new
        {
            x.UserId,
            x.IsRead
        });
        builder.HasIndex(x => new
        {
            x.UserId,
            x.ProjectId,
            x.IsRead
        });
    }
}