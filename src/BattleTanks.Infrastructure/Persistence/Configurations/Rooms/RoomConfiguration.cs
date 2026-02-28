using BattleTanks.Domain.Entities.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BattleTanks.Infrastructure.Persistence.Configurations.Rooms;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.MaxPlayers)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(r => r.Status);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasMany(r => r.RoomUsers)
            .WithOne(ru => ru.Room)
            .HasForeignKey(ru => ru.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.GameMap)
            .WithMany()
            .HasForeignKey(r => r.GameMapId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}