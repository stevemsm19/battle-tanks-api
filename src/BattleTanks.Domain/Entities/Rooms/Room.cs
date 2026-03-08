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
            throw new DomainException("Room name cannot be empty.");
            
        if (maxPlayers < 2 || maxPlayers > 4)
            throw new DomainException("Room capacity must be between 2 and 4 players.");

        if (map == null)
            throw new DomainException("Invalid map.");

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

    public void Join(User user, bool isOwner = false)
    {
        if (Status != RoomStatus.Open)
            throw new RoomClosedException();

        if (RoomUsers.Count >= MaxPlayers)
            throw new RoomFullException();

        if (RoomUsers.Any(ru => ru.UserId == user.Id))
            throw new UserAlreadyInRoomException(this.Id);

        RoomUsers.Add(new RoomUser
        {
            RoomId = this.Id,
            UserId = user.Id,
            IsOwner = isOwner
        });
    }

    public void Leave(User user)
    {
        var roomUser = RoomUsers.FirstOrDefault(ru => ru.UserId == user.Id);
        if (roomUser == null)
            throw new UserNotInRoomException();

        var wasOwner = roomUser.IsOwner;
        RoomUsers.Remove(roomUser);

        if (RoomUsers.Count == 0)
        {
            Status = RoomStatus.Cancelled;
        }
        else if (wasOwner)
        {
            var newOwner = RoomUsers.First();
            newOwner.IsOwner = true;
        }
    }
}
