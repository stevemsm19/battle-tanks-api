using BattleTanks.Domain.Interfaces.Rooms;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Interfaces.Maps;
using BattleTanks.Domain.Entities.Maps;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Enums;

using BattleTanks.Application.DTOs;
using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Exceptions;
using BattleTanks.Domain.Exceptions;

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

        //await ValidateUserNotInAnotherRoomAsync(user.Id);

        var mapId = CreateRoomRequestDTO.GameMapId;
        var map = await _gameMapRepository.GetByIdAsync(mapId) 
            ?? throw new InvalidGameMapException();

        var room = Room.Create(CreateRoomRequestDTO.Name, CreateRoomRequestDTO.MaxPlayers, map, user);

        await _roomRepository.AddAsync(room);
        var roomDto = _mapper.Map<RoomResponseDTO>(room);
        await _NotificationService.SendEventAsync("SearchRoom", "RoomCreated", roomDto);
        return roomDto;
    }

    public async Task<PagedResult<RoomResponseDTO>> GetAllRoomsAsync(RoomQueryParams queryParams)
    {
        var (items, totalCount) = await _roomRepository.GetAllAsync(queryParams.Page, queryParams.Size, queryParams.Status);
        var roomDTOs = items.Select(r => _mapper.Map<RoomResponseDTO>(r));
        return new PagedResult<RoomResponseDTO>(
            roomDTOs,
            totalCount,
            queryParams.Page,
            queryParams.Size
        );
    }

    public async Task<RoomWithPlayersResponseDTO?> GetRoomByIdAsync(Guid roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId)
            ?? throw new NotFoundException("Room not found");
        return _mapper.Map<RoomWithPlayersResponseDTO>(room);
    }

    public async Task JoinRoomAsync(Guid roomId)
    {
        var user = _userContext.CurrentUser!;

        var room = await _roomRepository.GetByIdAsync(roomId) 
            ?? throw new NotFoundException($"Room with ID {roomId} not found");

        await ValidateUserNotInAnotherRoomAsync(user.Id, room.Id);

        if (!room.Join(user)) return;
        
        await _roomRepository.UpdateAsync(room);

        await NotifyNewPlayerAsync(room, user);
    }

    public async Task LeaveRoomAsync(Guid roomId)
    {
        var user = _userContext.CurrentUser!;
        var room = await _roomRepository.GetByIdAsync(roomId)
            ?? throw new NotFoundException("Room not found.");

        room.Leave(user);

        await _roomRepository.UpdateAsync(room);

        await HandleLeaveNotificationsAsync(room, user);
    }

    private async Task ValidateUserNotInAnotherRoomAsync(Guid userId, Guid roomId)
    {
        var existingRoom = await _roomRepository.GetByUserIdAsync(userId);
        
        if (existingRoom != null && existingRoom.Id != roomId)
        {
            throw new UserAlreadyInRoomException(existingRoom.Id);
        }
    }

    private async Task NotifyNewPlayerAsync(Room room, User user)
    {
        var roomIdStr = room.Id.ToString();

        var playerJoinedTask = _NotificationService.SendEventAsync(roomIdStr, "PlayerJoined", new
        {
            id = user.Id.ToString(),
            name = user.Nickname
        });

        var countChangedTask = _NotificationService.SendEventAsync("SearchRoom", "RoomPlayerCountChanged", new
        {
            roomId = roomIdStr,
            currentPlayers = room.RoomUsers.Count
        });

        await Task.WhenAll(playerJoinedTask, countChangedTask);
    }

    private async Task HandleLeaveNotificationsAsync(Room room, User user)
    {
        var tasks = new List<Task>();

        if (room.Status == RoomStatus.Cancelled)
        {
            tasks.Add(_NotificationService.SendEventAsync("SearchRoom", "RoomStatusChanged", new { roomId = room.Id }));
        }
        else
        {
            var newOwnerId = room.RoomUsers.FirstOrDefault(ru => ru.IsOwner)?.UserId;

            tasks.Add(_NotificationService.SendEventAsync(room.Id.ToString(), "PlayerLeft", new
            {
                userId = user.Id.ToString(),
                newOwnerId = newOwnerId?.ToString()
            }));

            tasks.Add(_NotificationService.SendEventAsync("SearchRoom", "RoomPlayerCountChanged", new
            {
                roomId = room.Id.ToString(),
                currentPlayers = room.RoomUsers.Count
            }));
        }

        await Task.WhenAll(tasks);
    }
}

