using BattleTanks.Application.DTOs.Rooms;

namespace BattleTanks.Application.Interfaces.Rooms;

public interface IRoomService
{
    Task<IEnumerable<RoomDTO>> GetAllRoomsAsync();
    Task<RoomDTO> CreateRoomAsync(CreateRoomDTO createRoomDto);
    Task<RoomDetailDTO?> GetRoomByIdAsync(Guid roomId);
    Task<bool> JoinRoomAsync(Guid roomId);
    Task LeaveRoomAsync(Guid roomId);
}
