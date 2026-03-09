using System.Collections.Concurrent;

namespace BattleTanks.Domain.Entities.Game;

public class GameState
{
    public Guid RoomId { get; set; }
    public int[][] MapLayout { get; set; } = [];
    public ConcurrentDictionary<string, PlayerState> Players { get; set; } = new();
    public ConcurrentDictionary<Guid, BulletState> Bullets { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime LastAmmoReloadTime { get; set; } = DateTime.UtcNow;

    private int Rows => MapLayout.Length;
    private int Cols => Rows > 0 ? MapLayout[0].Length : 0;

    public GameState() { }

    public PlayerState? GetPlayer(string externalId)
    {
        return Players.GetValueOrDefault(externalId);
    }

    public PlayerState? FindPlayerAt(int x, int y, string excludeExternalId)
    {
        return Players.Values.FirstOrDefault(p =>
            p.ExternalId != excludeExternalId &&
            p.IsAlive &&
            p.IsAtPosition(x, y));
    }

    public HashSet<(int, int)> GetOccupiedCells()
    {
        return Players.Values
            .Where(p => p.IsAlive)
            .Select(p => ((int)p.X, (int)p.Y))
            .ToHashSet();
    }

    public bool CanPlayerFire(string externalId)
    {
        var player = GetPlayer(externalId);
        return player != null &&
               player.CanShoot &&
               !Bullets.Values.Any(b => b.OwnerExternalId == externalId);
    }

    public BulletState? CreateBullet(string externalId)
    {
        var player = GetPlayer(externalId);

        if (player == null || !player.CanShoot) return null;
        if (Bullets.Values.Any(b => b.OwnerExternalId == externalId)) return null;

        var bullet = BulletState.Create(player);
        player.ConsumeAmmo();
        Bullets.TryAdd(bullet.Id, bullet);

        return bullet;
    }

    public void RemoveBullet(Guid id) => Bullets.TryRemove(id, out _);

    public bool IsWithinBounds(int col, int row) =>
        row >= 0 && row < Rows && col >= 0 && col < Cols;

    public int GetCellType(int col, int row) => MapLayout[row][col];

    public bool IsWalkable(int col, int row) =>
        IsWithinBounds(col, row) && MapLayout[row][col] == 0;

    public void DestroyCell(int col, int row) => MapLayout[row][col] = 0;

    public void TryReloadAmmo()
    {
        if ((DateTime.UtcNow - LastAmmoReloadTime).TotalSeconds < 10) return;

        foreach (var player in Players.Values.Where(p => p.IsAlive && !p.HasAmmo))
        {
            player.ReloadAmmo();
        }

        LastAmmoReloadTime = DateTime.UtcNow;
    }

    public bool IsGameOver => Players.Values.Count(p => p.IsAlive) < 2;

    public PlayerState? GetWinner()
    {
        return Players.Values
            .OrderByDescending(p => p.Score)
            .ThenByDescending(p => p.Health)
            .FirstOrDefault();
    }
}