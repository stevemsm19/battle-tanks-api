using BattleTanks.Domain.Interfaces.Rooms;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Interfaces.Maps;
using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Enums;

using BattleTanks.Application.DTOs;
using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Exceptions;

using MapsterMapper;

namespace BattleTanks.Application.Services.Rooms;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IGameMapRepository _gameMapRepository;
    private readonly INotificationService _NotificationService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IGameMapRepository gameMapRepository, INotificationService NotificationService, IUserContext userContext, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _gameMapRepository = gameMapRepository;
        _NotificationService = NotificationService;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<RoomResponseDTO> CreateRoomAsync(CreateRoomRequestDTO CreateRoomRequestDTO)
    {
        var user = _userContext.CurrentUser!;

        await ValidateUserNotInAnotherRoomAsync(user.Id);

        var mapId = CreateRoomRequestDTO.GameMapId;
        var map = await _gameMapRepository.GetByIdAsync(mapId);
        if (map == null) throw new BadRequestException("Invalid map ID");

        var room = _mapper.From(CreateRoomRequestDTO)
                          .AddParameters("User", user)
                          .AdaptToType<Room>();

        await _roomRepository.AddAsync(room);
        var roomDto = _mapper.Map<RoomResponseDTO>(room);
        await _NotificationService.SendEventAsync("SearchRoom", "RoomCreated", roomDto);
        return roomDto;
    }

    public async Task<PagedResult<RoomResponseDTO>> GetAllRoomsAsync(RoomQueryParams queryParams)
    {
        var (items, totalCount) = await _roomRepository.GetAllAsync(queryParams.Page, queryParams.Size, queryParams.Status);
        var roomDTOs = items.Select(r => _mapper.Map<RoomResponseDTO>(r));
        return new PagedResult<RoomResponseDTO>
        {
            Items = roomDTOs,
            TotalCount = totalCount,
            Page = queryParams.Page,
            Size = queryParams.Size
        };
    }

    public async Task<RoomWithPlayersResponseDTO?> GetRoomByIdAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null) throw new NotFoundException("Room not found");
        return _mapper.Map<RoomWithPlayersResponseDTO>(room);
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

        await _NotificationService.SendEventAsync(roomId.ToString(), "PlayerJoined", new
        {
            id = user.Id.ToString(),
            name = user.Nickname
        });

        await _NotificationService.SendEventAsync("SearchRoom", "RoomPlayerCountChanged", new
        {
            roomId = room.Id.ToString(),
            currentPlayers = room.RoomUsers.Count
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
            room.Status = RoomStatus.Cancelled;
            await _roomRepository.UpdateAsync(room);
            await _NotificationService.SendEventAsync("SearchRoom", "RoomStatusChanged", new { roomId = room.Id });
            return;
        }

        if (wasOwner)
        {
            var newOwner = room.RoomUsers.First();
            newOwner.IsOwner = true;
        }

        await _roomRepository.UpdateAsync(room);

        var newOwnerId = wasOwner ? room.RoomUsers.First().UserId : (Guid?)null;
        await _NotificationService.SendEventAsync(room.Id.ToString(), "PlayerLeft", new
        {
            userId = user.Id.ToString(),
            newOwnerId = newOwnerId?.ToString()
        });

        await _NotificationService.SendEventAsync("SearchRoom", "RoomPlayerCountChanged", new
        {
            roomId = room.Id.ToString(),
            currentPlayers = room.RoomUsers.Count
        });
    }

    private async Task ValidateUserNotInAnotherRoomAsync(Guid userId, Guid? joiningRoomId = null)
    {
        var existingRoom = await _roomRepository.GetByUserIdAsync(userId);
        
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
