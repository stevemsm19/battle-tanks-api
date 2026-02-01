using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Domain.Entities.Rooms;

public class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; } = 4;
    public ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();
    public bool IsOpen { get; set; } = true;
}
