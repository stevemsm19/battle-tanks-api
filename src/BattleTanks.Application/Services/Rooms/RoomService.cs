using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Interfaces.Rooms;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Enums;
using BattleTanks.Application.Common.Interfaces;

// TODO: Exception Handling
namespace BattleTanks.Application.Services.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameNotificationService _gameNotificationService;

    public RoomService(IRoomRepository roomRepository, IUserRepository userRepository, IGameNotificationService gameNotificationService)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _gameNotificationService = gameNotificationService;
    }

    public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(RoomDTO.FromEntity);
    }

    public async Task<RoomDetailDTO?> GetRoomByIdAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null) return null;
        return RoomDetailDTO.FromEntity(room);
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
        var roomDto = RoomDTO.FromEntity(room);
        await _gameNotificationService.SendGameEventAsync("SearchRoom", "RoomCreated", roomDto);
        return roomDto;
    }

    public async Task<bool> JoinRoomAsync(Guid roomId, string firebaseUid)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new Exception("Room not found");

        if (room.Status != RoomStatus.Open)
            throw new Exception("Room is closed or cancelled");

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

        await _gameNotificationService.SendGameEventAsync(roomId.ToString(), "PlayerJoined", new
        {
            id = user.Id.ToString(),
            name = user.Nickname,
            status = "waiting",
            x = 0,
            y = 0
        });

        return true;
    }
}
