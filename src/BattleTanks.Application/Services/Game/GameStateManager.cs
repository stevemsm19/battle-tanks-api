using System.Collections.Concurrent;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Domain.Entities.Game;
using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Application.Services.Game;

public class GameStateManager : IGameStateManager
{
    private readonly ConcurrentDictionary<Guid, GameState> _activeGames = new();

    public GameState InitializeGame(Guid roomId, GameMap map, IEnumerable<RoomUser> players)
    {
        var gameState = new GameState { RoomId = roomId, MapLayout = map.Layout };
        var random = new Random();
        var validCells = GetValidCells(map.Layout);

        foreach (var roomUser in players)
        {
            var (startX, startY) = GetRandomSpawnPoint(validCells, random);

            var player = PlayerState.Create(
                userId: roomUser.User.Id.ToString(),
                externalId: roomUser.User.ExternalId,
                name: roomUser.User.Nickname,
                x: startX,
                y: startY
            );

            gameState.Players.TryAdd(player.ExternalId, player);
        }

        _activeGames.TryAdd(roomId, gameState);
        return gameState;
    }

    public BulletState? SpawnBullet(Guid roomId, string externalId)
    {
        return _activeGames.TryGetValue(roomId, out var gameState) 
            ? gameState.CreateBullet(externalId) 
            : null;
    }

    public void SetPlayerDirection(Guid roomId, string externalId, string direction)
    {
        if (_activeGames.TryGetValue(roomId, out var gameState))
        {
            gameState.GetPlayer(externalId)?.UpdateDirection(direction);
        }
    }

    private (float X, float Y) GetRandomSpawnPoint(List<(int X, int Y)> cells, Random rnd)
    {
        if (cells.Count == 0) return (1f, 1f);
        var index = rnd.Next(cells.Count);
        var p = cells[index];
        cells.RemoveAt(index);
        return (p.X + 1, p.Y + 1);
    }

    private List<(int X, int Y)> GetValidCells(int[][] layout)
    {
        return layout.SelectMany((row, y) => 
            row.Select((cell, x) => (cell, x, y))
               .Where(c => c.cell == 0)
               .Select(c => (c.x, c.y))
        ).ToList();
    }

    public GameState? GetGameState(Guid id) => _activeGames.GetValueOrDefault(id);
    public IEnumerable<GameState> GetAllActiveGames() => _activeGames.Values;
    public void RemoveGame(Guid id) => _activeGames.TryRemove(id, out _);
}
