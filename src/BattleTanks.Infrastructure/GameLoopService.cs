using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Domain.Entities.Game;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BattleTanks.Infrastructure;

public class GameLoopService : BackgroundService
{
    private const int TimeMs = 300;

    private readonly IGameStateManager _gameStateManager;
    private readonly IServiceScopeFactory _scopeFactory;

    public GameLoopService(IGameStateManager gameStateManager, IServiceScopeFactory scopeFactory)
    {
        _gameStateManager = gameStateManager;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(TimeMs));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var activeGames = _gameStateManager.GetAllActiveGames().Where(g => g.IsActive).ToList();

            foreach (var game in activeGames)
                AdvancePlayers(game);

            var tasks = activeGames.Select(g => notifier.SendEventAsync(g.RoomId.ToString(), "GameTick", new
            {
                players = g.Players.Values.Select(p => new
                {
                    userId    = p.UserId,
                    name      = p.Name,
                    x         = p.X,
                    y         = p.Y,
                    direction = p.Direction,
                    health    = p.Health
                })
            }));

            await Task.WhenAll(tasks);
        }
    }

    private static void AdvancePlayers(GameState game)
    {
        var map  = game.MapLayout;
        int rows = map.Length;
        int cols = rows > 0 ? map[0].Length : 0;

        var players = game.Players.Values.ToList();
        var occupiedCells = players
            .Select(p => ((int)p.X, (int)p.Y))
            .ToHashSet();

        foreach (var player in players)
        {
            if (player.Direction == "NONE") continue;

            var (dx, dy) = player.Direction switch
            {
                "UP"    => ( 0, -1),
                "DOWN"  => ( 0,  1),
                "LEFT"  => (-1,  0),
                "RIGHT" => ( 1,  0),
                _       => ( 0,  0)
            };

            if (dx == 0 && dy == 0) continue;

            var nextX = (int)player.X + dx;
            var nextY = (int)player.Y + dy;

            var mapCol = nextX - 1;
            var mapRow = nextY - 1;

            if (mapRow < 0 || mapRow >= rows || mapCol < 0 || mapCol >= cols) continue;

            if (map[mapRow][mapCol] != 0) continue;

            if (occupiedCells.Contains((nextX, nextY))) continue;

            occupiedCells.Remove(((int)player.X, (int)player.Y));
            occupiedCells.Add((nextX, nextY));

            player.X = nextX;
            player.Y = nextY;
        }
    }
}
