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
        var gameState = new GameState
        {
            RoomId = roomId,
            MapLayout = map.Layout
        };

        var random = new Random();
        var validCells = GetValidCells(map.Layout);

        foreach (var roomUser in players)
        {
            float startX = 1f;
            float startY = 1f;

            if (validCells.Count > 0)
            {
                var index = random.Next(validCells.Count);
                var point = validCells[index];
                validCells.RemoveAt(index);
                
                startX = point.X + 1;
                startY = point.Y + 1;
            }

            var playerState = new PlayerState
            {
                UserId = roomUser.User.Id.ToString(),
                FirebaseUid = roomUser.User.FirebaseUid,
                Name = roomUser.User.Nickname,
                X = startX,
                Y = startY,
                Direction = "NONE",
                Health = 100
            };

            string playerId = playerState.UserId;
            PlayerState initialPlayerState = playerState;

            gameState.Players.TryAdd(playerId, initialPlayerState);
        }

        _activeGames.AddOrUpdate(roomId, gameState, (existingId, oldState) => gameState);

        return gameState;
    }

    public GameState? GetGameState(Guid roomId)
    {
        _activeGames.TryGetValue(roomId, out var foundGameState);
        return foundGameState;
    }

    public IEnumerable<GameState> GetAllActiveGames() => _activeGames.Values;

    public void RemoveGame(Guid roomId)
    {
        _activeGames.TryRemove(roomId, out _);
    }

    public void SetPlayerDirection(Guid roomId, string firebaseUid, string direction)
    {
        if (!_activeGames.TryGetValue(roomId, out var gameState)) return;

        var player = gameState.Players.Values.FirstOrDefault(p => p.FirebaseUid == firebaseUid);
        if (player != null)
            player.Direction = direction;
    }

    private List<(int X, int Y)> GetValidCells(int[][] mapLayout)
    {
        var validCells = new List<(int X, int Y)>();
        
        for (int y = 0; y < mapLayout.Length; y++)
        {
            for (int x = 0; x < mapLayout[y].Length; x++)
            {
                if (mapLayout[y][x] == 0)
                {
                    validCells.Add((x, y));
                }
            }
        }
        
        return validCells;
    }
}
