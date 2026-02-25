using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Enums;

namespace BattleTanks.Domain.Entities.Rooms;

public class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; } = 4;
    public ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
    public RoomStatus Status { get; set; } = RoomStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
