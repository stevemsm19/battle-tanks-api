using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Application.DTOs.Rooms;

public record RoomDetailDTO(Guid RoomId, string RoomName, IEnumerable<RoomPlayerDTO> Players)
{
    public static RoomDetailDTO FromEntity(Room room)
    {
        return new RoomDetailDTO(
            room.Id,
            room.Name,
            room.RoomUsers.Select(ru => new RoomPlayerDTO(
                ru.User.Id.ToString(),
                ru.User.Nickname,
                ru.IsOwner
            ))
        );
    }
}

public record RoomPlayerDTO(string Id, string Name, bool IsOwner);
