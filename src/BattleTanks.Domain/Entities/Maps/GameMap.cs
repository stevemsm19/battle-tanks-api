namespace BattleTanks.Domain.Entities.Maps;

public class GameMap
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int[][] Layout { get; set; } = [];
}
