using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Interfaces.Rooms;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Enums;
using BattleTanks.Application.Common.Interfaces;
using BattleTanks.Application.Exceptions;

namespace BattleTanks.Application.Services.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameNotificationService _gameNotificationService;
    private readonly IUserContext _userContext;

    public RoomService(IRoomRepository roomRepository, IUserRepository userRepository, IGameNotificationService gameNotificationService, IUserContext userContext)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _gameNotificationService = gameNotificationService;
        _userContext = userContext;
    }

    public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return rooms.Select(RoomDTO.FromEntity);
    }

    public async Task<RoomDetailDTO?> GetRoomByIdAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null) throw new NotFoundException("Room not found");
        return RoomDetailDTO.FromEntity(room);
    }

    public async Task<RoomDTO> CreateRoomAsync(CreateRoomDTO createRoomDto)
    {
        var user = _userContext.CurrentUser!;

        await ValidateUserNotInAnotherRoomAsync(user.Id);

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = createRoomDto.Name,
            MaxPlayers = createRoomDto.MaxPlayers,
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

    public async Task<bool> JoinRoomAsync(Guid roomId)
    {
        var user = _userContext.CurrentUser!;

        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new NotFoundException("Room not found");

        if (room.Status != RoomStatus.Open)
            throw new BadRequestException("Room is closed or cancelled");

        await ValidateUserNotInAnotherRoomAsync(user.Id, roomId);

        if (room.RoomUsers.Count >= room.MaxPlayers)
            throw new BadRequestException("Room is full");

        room.RoomUsers.Add(new RoomUser
        {
            RoomId = room.Id,
            UserId = user.Id,
            IsOwner = false,
            IsReady = true
        });
        
        await _roomRepository.UpdateAsync(room);

        await _gameNotificationService.SendGameEventAsync(roomId.ToString(), "PlayerJoined", new
        {
            id = user.Id.ToString(),
            name = user.Nickname,
            status = "waiting"
        });

        return true;
    }

    public async Task LeaveRoomAsync(Guid roomId)
    {
        var user = _userContext.CurrentUser!;

        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new NotFoundException("Room not found.");

        var roomUser = room.RoomUsers.FirstOrDefault(ru => ru.UserId == user.Id);
        if (roomUser == null)
            throw new BadRequestException("User does not belong to this room.");

        var wasOwner = roomUser.IsOwner;

        room.RoomUsers.Remove(roomUser);

        if (room.RoomUsers.Count == 0)
        {
            await _roomRepository.DeleteAsync(room);
            await _gameNotificationService.SendGameEventAsync("SearchRoom", "RoomDeleted", new { roomId = room.Id });
            return;
        }

        if (wasOwner)
        {
            var newOwner = room.RoomUsers.First();
            newOwner.IsOwner = true;
        }

        await _roomRepository.UpdateAsync(room);

        var newOwnerId = wasOwner ? room.RoomUsers.First().UserId : (Guid?)null;
        await _gameNotificationService.SendGameEventAsync(room.Id.ToString(), "PlayerLeft", new
        {
            userId = user.Id.ToString(),
            newOwnerId = newOwnerId?.ToString()
        });
    }

    private async Task ValidateUserNotInAnotherRoomAsync(Guid userId, Guid? joiningRoomId = null)
    {
        var existingRoom = await _roomRepository.GetRoomByUserIdAsync(userId);
        
        if (existingRoom != null)
        {
            if (joiningRoomId.HasValue && existingRoom.Id == joiningRoomId.Value)
            {
                throw new BadRequestException("User is already in this room.");
            }
            
            throw new BadRequestException($"User belongs to another room:{existingRoom.Id}");
        }
    }
}
