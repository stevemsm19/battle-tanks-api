using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Domain.Entities.Rooms;

public class RoomUser
{
    public Guid RoomId { get; private set; }
    public Room Room { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public bool IsReady { get; private set; }
    public bool IsOwner { get; private set; }

    protected RoomUser() { }

    public static RoomUser Create(Guid roomId, Guid userId, bool isOwner)
    {
        return new RoomUser
        {
            RoomId = roomId,
            UserId = userId,
            IsOwner = isOwner,
            IsReady = true
        };
    }

    public void PromoteToOwner() => IsOwner = true;
}