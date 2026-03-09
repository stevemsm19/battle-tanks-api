using BattleTanks.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BattleTanks.Infrastructure.Persistence.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);
        
        builder.HasIndex(u => u.ExternalId)
            .IsUnique();
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Property(u => u.ExternalId)
            .IsRequired()
            .HasMaxLength(128);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.Nickname)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(2048);
        
        builder.HasMany(u => u.RoomUsers)
            .WithOne(ru => ru.User)
            .HasForeignKey(ru => ru.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(u => u.Score)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(u => u.Score);

        builder.Property(u => u.CreatedAt)
            .IsRequired();
        
        builder.Property(u => u.UpdatedAt)
            .IsRequired();
    }
}
