using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Entities.Maps;
using Microsoft.EntityFrameworkCore;

namespace BattleTanks.Infrastructure.Persistence;

public class BattleTanksDbContext : DbContext
{
    public BattleTanksDbContext(DbContextOptions<BattleTanksDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomUser> RoomUsers { get; set; }
    public DbSet<GameMap> GameMaps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BattleTanksDbContext).Assembly);
    }
}
