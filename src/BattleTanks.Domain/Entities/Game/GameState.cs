using System.Collections.Concurrent;

namespace BattleTanks.Domain.Entities.Game;

public class GameState
{
    public Guid RoomId { get; set; }
    public int[][] MapLayout { get; set; } = [];
    public ConcurrentDictionary<string, PlayerState> Players { get; set; } = new();
    public bool IsActive { get; set; } = true;
}
