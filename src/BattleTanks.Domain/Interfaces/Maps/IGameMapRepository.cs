using BattleTanks.Domain.Entities.Maps;

namespace BattleTanks.Domain.Interfaces.Maps;

public interface IGameMapRepository
{
    Task<GameMap?> GetByIdAsync(Guid id);
    Task<IEnumerable<GameMap>> GetAllAsync();
}
