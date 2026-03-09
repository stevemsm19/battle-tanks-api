namespace BattleTanks.Domain.Entities.Game;

public class BulletState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int X { get; set; }  
    public int Y { get; set; }  
    public string Direction { get; set; } = "UP";
    public string OwnerExternalId { get; set; } = string.Empty;

    public BulletState() { }

    public static BulletState Create(PlayerState player)
    {
        return new BulletState
        {
            Id = Guid.NewGuid(),
            X = (int)player.X,
            Y = (int)player.Y,
            Direction = player.LastValidDirection,
            OwnerExternalId = player.ExternalId
        };
    }

    public (int X, int Y) GetMovementStep()
    {
        return Direction switch
        {
            "UP" => ( 0, -1),
            "DOWN" => ( 0,  1),
            "LEFT" => (-1,  0),
            "RIGHT" => ( 1,  0),
            _ => ( 0,  0)
        };
    }

    public void Advance(int dx, int dy)
    {
        X += dx;
        Y += dy;
    }
}
