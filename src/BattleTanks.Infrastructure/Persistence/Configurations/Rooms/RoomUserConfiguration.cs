using BattleTanks.Domain.Entities.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BattleTanks.Infrastructure.Persistence.Configurations.Rooms;

public class RoomUserConfiguration : IEntityTypeConfiguration<RoomUser>
{
    public void Configure(EntityTypeBuilder<RoomUser> builder)
    {
        builder.ToTable("room_users");

        builder.HasKey(ru => new { ru.RoomId, ru.UserId });

        builder.HasOne(ru => ru.Room)
            .WithMany(r => r.RoomUsers)
            .HasForeignKey(ru => ru.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ru => ru.User)
            .WithMany(u => u.RoomUsers)
            .HasForeignKey(ru => ru.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(ru => ru.IsReady)
            .HasDefaultValue(false);
            
        builder.Property(ru => ru.IsOwner)
            .HasDefaultValue(false);
    }
}