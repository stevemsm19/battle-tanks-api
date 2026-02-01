using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Interfaces.Rooms;
using BattleTanks.Domain.Interfaces.Users;

namespace BattleTanks.Application.Services.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    public RoomService(IRoomRepository roomRepository, IUserRepository userRepository)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(RoomDTO.FromEntity);
    }

    public async Task<RoomDTO> CreateRoomAsync(CreateRoomDTO createRoomDto, string firebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(firebaseUid);
        if (user == null)
            throw new Exception("User not found");

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = createRoomDto.Name,
            MaxPlayers = 4,
            RoomUsers = new List<RoomUser>()
        };

        room.RoomUsers.Add(new RoomUser
        {
            RoomId = room.Id,
            UserId = user.Id,
            IsOwner = true,
            IsReady = true
        });

        await _roomRepository.AddAsync(room);
        return RoomDTO.FromEntity(room);
    }

    public async Task<bool> JoinRoomAsync(Guid roomId, string firebaseUid)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new Exception("Room not found");

        if (!room.IsOpen)
            throw new Exception("Room is closed");

        var user = await _userRepository.GetByFirebaseUidAsync(firebaseUid);
        if (user == null)
            throw new Exception("User not found");
        if (room.RoomUsers.Any(ru => ru.UserId == user.Id))
            return true;

        if (room.RoomUsers.Count >= room.MaxPlayers)
            throw new Exception("Room is full");

        room.RoomUsers.Add(new RoomUser
        {
            RoomId = room.Id,
            UserId = user.Id,
            IsOwner = false,
            IsReady = false
        });
        
        await _roomRepository.UpdateAsync(room);

        return true;
    }
}
