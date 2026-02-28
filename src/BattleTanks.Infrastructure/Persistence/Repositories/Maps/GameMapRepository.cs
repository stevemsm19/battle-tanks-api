using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Interfaces.Maps;
using Microsoft.EntityFrameworkCore;

namespace BattleTanks.Infrastructure.Persistence.Repositories.Maps;

public class GameMapRepository : IGameMapRepository
{
    private readonly BattleTanksDbContext _dbContext;

    public GameMapRepository(BattleTanksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GameMap?> GetByIdAsync(Guid id)
    {
        return await _dbContext.GameMaps.FindAsync(id);
    }

    public async Task<IEnumerable<GameMap>> GetAllAsync()
    {
        return await _dbContext.GameMaps.AsNoTracking().ToListAsync();
    }
}
