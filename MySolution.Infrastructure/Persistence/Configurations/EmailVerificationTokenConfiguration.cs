using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Configurations;

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.ToTable("EmailVerificationTokens");
        
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()");
        builder.Property(t => t.ExpiresAt)
            .IsRequired();
        
        builder.HasOne(t => t.User)
            .WithMany(x => x.EmailVerificationTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}