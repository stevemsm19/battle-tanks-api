using BattleTanks.Application.DTOs.Maps;

namespace BattleTanks.Application.Interfaces.Maps;

public interface IGameMapService
{
    Task<IEnumerable<GameMapDTO>> GetAllMapsAsync();
}
