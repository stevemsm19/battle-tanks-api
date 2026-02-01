using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Domain.Entities.Rooms;

public class RoomUser
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsReady { get; set; }
    public bool IsOwner { get; set; }
}
