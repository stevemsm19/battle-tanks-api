using BattleTanks.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace BattleTanks.Infrastructure.Persistence;

public class BattleTanksDbContext : DbContext
{
    public BattleTanksDbContext(DbContextOptions<BattleTanksDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<User> Users { get; set; }
}
