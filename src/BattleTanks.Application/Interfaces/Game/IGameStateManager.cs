using BattleTanks.Domain.Entities.Game;
using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Application.Interfaces.Game;

public interface IGameStateManager
{
    GameState InitializeGame(Guid roomId, GameMap map, IEnumerable<RoomUser> players);
    GameState? GetGameState(Guid roomId);
    IEnumerable<GameState> GetAllActiveGames();
    void RemoveGame(Guid roomId);
    void SetPlayerDirection(Guid roomId, string externalId, string direction);
    BulletState? SpawnBullet(Guid roomId, string externalId);
}
