using System.ComponentModel.DataAnnotations;
using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Application.DTOs.Rooms;

public record CreateRoomRequestDTO(
    [Required, StringLength(50, MinimumLength = 3)] string Name,
    [Range(2, 4)] int MaxPlayers,
    [Required] Guid GameMapId
);

public record RoomResponseDTO(
    Guid Id,
    string Name,
    int MaxPlayers,
    int CurrentPlayers,
    string Status,
    Guid GameMapId,
    string GameMapName
);

public record RoomPlayerResponseDTO(
    string Id, 
    string FirebaseUid, 
    string Name, 
    bool IsOwner
);

public record RoomWithPlayersResponseDTO(
    RoomResponseDTO Room,
    IEnumerable<RoomPlayerResponseDTO> Players
);