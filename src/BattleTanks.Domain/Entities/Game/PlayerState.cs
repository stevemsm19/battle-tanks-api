namespace BattleTanks.Domain.Entities.Game;

public class PlayerState
{
    public string UserId { get; set; } = string.Empty;
    public string FirebaseUid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public string Direction { get; set; } = "UP";
    public int Health { get; set; } = 100;
}
