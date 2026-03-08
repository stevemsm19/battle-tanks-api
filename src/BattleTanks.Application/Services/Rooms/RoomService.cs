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
        var map = await _gameMapRepository.GetByIdAsync(mapId);
        if (map == null) throw new DomainException("Invalid map ID");

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

        await ValidateUserNotInAnotherRoomAsync(user.Id);

        room.Join(user);
        
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

        var wasOwner = room.RoomUsers.FirstOrDefault(ru => ru.UserId == user.Id)?.IsOwner ?? false;

        room.Leave(user);

        if (room.Status == RoomStatus.Cancelled)
        {
            await _roomRepository.UpdateAsync(room);
            await _NotificationService.SendEventAsync("SearchRoom", "RoomStatusChanged", new { roomId = room.Id });
            return;
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

    private async Task ValidateUserNotInAnotherRoomAsync(Guid userId)
    {
        var existingRoom = await _roomRepository.GetByUserIdAsync(userId);
        
        if (existingRoom != null)
        {
            throw new UserAlreadyInRoomException(existingRoom.Id);
        }
    }
}
