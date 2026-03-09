using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Domain.Entities.Game;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Infrastructure.Services.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BattleTanks.Infrastructure;

public class GameLoopService : BackgroundService
{
    private const int TickMs = 300;
    private const int DamagePerHit = 25;
    private const int ScorePerKill = 20;
    private const int ScorePerDestroy = 10;
    private const int BulletStepsPerTick = 2;

    private readonly IGameStateManager _gameStateManager;
    private readonly IServiceScopeFactory _scopeFactory;

    public GameLoopService(IGameStateManager gameStateManager, IServiceScopeFactory scopeFactory)
    {
        _gameStateManager = gameStateManager;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(TickMs));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var leaderboard = scope.ServiceProvider.GetRequiredService<ILeaderboardService>();

            var activeGames = _gameStateManager.GetAllActiveGames().Where(g => g.IsActive).ToList();
            var tasks = new List<Task>();

            foreach (var game in activeGames)
            {
                AdvancePlayers(game);
                bool mapChanged = AdvanceBullets(game);
                game.TryReloadAmmo();

                tasks.Add(SendGameTick(notifier, game));

                if (mapChanged)
                    tasks.Add(notifier.SendEventAsync(game.RoomId.ToString(), "MapUpdated", game.MapLayout));

                if (game.IsGameOver)
                    tasks.Add(HandleGameOver(scope, notifier, leaderboard, game));

            }

            await Task.WhenAll(tasks);
        }
    }

    private static void AdvancePlayers(GameState game)
    {
        var occupiedCells = game.GetOccupiedCells();

        foreach (var player in game.Players.Values.ToList())
        {
            if (!player.IsMoving) continue;

            var (dx, dy) = player.GetMovementStep();
            if (dx == 0 && dy == 0) continue;

            var nextX = (int)player.X + dx;
            var nextY = (int)player.Y + dy;

            if (!game.IsWalkable(nextX - 1, nextY - 1)) continue;
            if (occupiedCells.Contains((nextX, nextY))) continue;

            occupiedCells.Remove(((int)player.X, (int)player.Y));
            occupiedCells.Add((nextX, nextY));
            player.MoveTo(nextX, nextY);
        }
    }

    private static bool AdvanceBullets(GameState game)
    {
        bool mapChanged = false;

        foreach (var (id, bullet) in game.Bullets.ToList())
        {
            var (dx, dy) = bullet.GetMovementStep();

            for (int step = 0; step < BulletStepsPerTick; step++)
            {
                var nextX = bullet.X + dx;
                var nextY = bullet.Y + dy;
                var col = nextX - 1;
                var row = nextY - 1;

                if (!game.IsWithinBounds(col, row))
                {
                    game.RemoveBullet(id);
                    break;
                }

                var hitPlayer = game.FindPlayerAt(nextX, nextY, bullet.OwnerExternalId);
                if (hitPlayer != null)
                {
                    hitPlayer.TakeDamage(DamagePerHit);
                    game.GetPlayer(bullet.OwnerExternalId)?.AddScore(ScorePerKill);
                    game.RemoveBullet(id);
                    break;
                }

                var cell = game.GetCellType(col, row);

                if (cell == 1)
                {
                    game.RemoveBullet(id);
                    break;
                }

                if (cell == 2)
                {
                    game.DestroyCell(col, row);
                    game.GetPlayer(bullet.OwnerExternalId)?.AddScore(ScorePerDestroy);
                    game.RemoveBullet(id);
                    mapChanged = true;
                    break;
                }

                bullet.Advance(dx, dy);
            }
        }

        return mapChanged;
    }

    private static Task SendGameTick(INotificationService notifier, GameState game)
    {
        var payload = new
        {
            players = game.Players.Values.Select(p => new
            {
                userId = p.UserId,
                name = p.Name,
                x = p.X,
                y = p.Y,
                direction = p.Direction,
                health = p.Health,
                ammo = p.Ammo,
                score = p.Score,
                externalId = p.ExternalId
            }),
            bullets = game.Bullets.Values.Select(b => new
            {
                id = b.Id,
                x = b.X,
                y = b.Y,
                direction = b.Direction
            })
        };

        return notifier.SendEventAsync(game.RoomId.ToString(), "GameTick", payload);
    }

    private async Task HandleGameOver(
        IServiceScope scope, INotificationService notifier,
        ILeaderboardService leaderboard, GameState game)
    {
        var winner = game.GetWinner();

        await notifier.SendEventAsync(game.RoomId.ToString(), "GameOver", new
        {
            winnerName = winner?.Name,
            winnerScore = winner?.Score ?? 0,
            winnerHealth = winner?.Health ?? 0
        });

        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        foreach (var p in game.Players.Values)
        {
            var user = await userRepository.GetByExternalIdAsync(p.ExternalId);
            if (user != null)
            {
                user.Score += p.Score;
                userRepository.Update(user);
                await leaderboard.UpdateScoreAsync(user.Nickname, user.Score);
            }
        }

        await userRepository.SaveChangesAsync();
        _gameStateManager.RemoveGame(game.RoomId);
    }
}