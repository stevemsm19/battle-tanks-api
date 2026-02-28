using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Enums;

namespace BattleTanks.Domain.Entities.Rooms;

public class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; } = 4;
    public RoomStatus Status { get; set; } = RoomStatus.Open;
    public Guid GameMapId { get; set; }
    public GameMap GameMap { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
}
