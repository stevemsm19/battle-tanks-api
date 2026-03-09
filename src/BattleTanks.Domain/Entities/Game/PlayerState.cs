namespace BattleTanks.Domain.Entities.Game;

public class PlayerState
{
    public string UserId { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public string Direction { get; set; } = "NONE";
    public string LastValidDirection { get; set; } = "UP";
    public int Health { get; set; } = 100;
    public int Ammo { get; set; } = 10;
    public int Score { get; set; } = 0;

    public PlayerState() { }

    public static PlayerState Create(string userId, string externalId, string name, float x, float y)
    {
        return new PlayerState
        {
            UserId = userId,
            ExternalId = externalId,
            Name = name,
            X = x,
            Y = y
        };
    }

    public bool IsAlive => Health > 0;
    public bool HasAmmo => Ammo > 0;
    public bool CanShoot => IsAlive && HasAmmo;
    public bool IsMoving => IsAlive && Direction != "NONE";

    public (int X, int Y) GetMovementStep()
    {
        return Direction switch
        {
            "UP" => ( 0, -1),
            "DOWN" => ( 0, 1),
            "LEFT" => (-1, 0),
            "RIGHT" => ( 1, 0),
            _ => ( 0, 0)
        };
    }

    public void UpdateDirection(string newDirection)
    {
        if (!IsAlive) return;

        Direction = newDirection;
        if (newDirection != "NONE")
        {
            LastValidDirection = newDirection;
        }
    }

    public void MoveTo(float x, float y)
    {
        X = x;
        Y = y;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
    }

    public void AddScore(int points) => Score += points;

    public void ConsumeAmmo() { Ammo--; }

    public void ReloadAmmo() { Ammo = 10; }

    public bool IsAtPosition(int x, int y) => (int)X == x && (int)Y == y;
}