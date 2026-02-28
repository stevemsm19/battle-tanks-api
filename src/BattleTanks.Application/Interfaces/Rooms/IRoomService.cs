using BattleTanks.Application.DTOs;
using BattleTanks.Application.DTOs.Rooms;

namespace BattleTanks.Application.Interfaces.Rooms;

public interface IRoomService
{
    Task<RoomResponseDTO> CreateRoomAsync(CreateRoomRequestDTO CreateRoomRequestDTO);
    Task<RoomWithPlayersResponseDTO?> GetRoomByIdAsync(Guid roomId);
    Task<PagedResult<RoomResponseDTO>> GetAllRoomsAsync(RoomQueryParams queryParams);
    Task<bool> JoinRoomAsync(Guid roomId);
    Task LeaveRoomAsync(Guid roomId);
}
