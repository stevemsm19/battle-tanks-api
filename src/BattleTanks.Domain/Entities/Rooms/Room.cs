using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Enums;
using BattleTanks.Domain.Exceptions;

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

    public Room() { }

    public static Room Create(string name, int maxPlayers, GameMap map, User owner)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidRoomNameException();
            
        if (maxPlayers < 2 || maxPlayers > 4)
            throw new InvalidRoomCapacityException();

        if (map == null)
            throw new InvalidGameMapException();

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = name,
            MaxPlayers = maxPlayers,
            GameMapId = map.Id,
            GameMap = map,
            Status = RoomStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        room.Join(owner, isOwner: true);

        return room;
    }

    public bool Join(User user, bool isOwner = false)
    {
        if (RoomUsers.Any(ru => ru.UserId == user.Id))
        {
            return false;
        }

        if (Status != RoomStatus.Open)
            throw new RoomClosedException();

        if (RoomUsers.Count >= MaxPlayers)
            throw new RoomFullException();

        var roomUser = RoomUser.Create(this.Id, user.Id, isOwner);
        RoomUsers.Add(roomUser);

        return true;
    }

    public void Leave(User user)
    {
        var roomUser = RoomUsers.FirstOrDefault(ru => ru.UserId == user.Id) 
            ?? throw new UserNotInRoomException();

        var wasOwner = roomUser.IsOwner;
        RoomUsers.Remove(roomUser);

        if (RoomUsers.Count > 0 && wasOwner)
        {
            RoomUsers.First().PromoteToOwner(); 
        }
        else if (RoomUsers.Count == 0)
        {
            Status = RoomStatus.Cancelled;
        }
    }

    public void Start(Guid currentUserId)
    {
        if (Status != RoomStatus.Open)
            throw new RoomNotOpenException();

        var owner = RoomUsers.FirstOrDefault(ru => ru.IsOwner);
        if (owner == null || owner.UserId != currentUserId)
            throw new NotRoomOwnerException();

        if (RoomUsers.Count < 2)
            throw new InsufficientPlayersException();

        if (GameMap == null)
            throw new MissingMapDataException();

        Status = RoomStatus.Closed;
    }
}
