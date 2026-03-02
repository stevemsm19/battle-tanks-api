using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Exceptions;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Domain.Enums;
using BattleTanks.Domain.Interfaces.Rooms;

namespace BattleTanks.Application.Services.Game;

public class GameSessionService : IGameSessionService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserContext _userContext;
    private readonly IGameStateManager _gameStateManager;
    private readonly INotificationService _NotificationService;

    public GameSessionService(
        IRoomRepository roomRepository,
        IUserContext userContext,
        IGameStateManager gameStateManager,
        INotificationService NotificationService)
    {
        _roomRepository = roomRepository;
        _userContext = userContext;
        _gameStateManager = gameStateManager;
        _NotificationService = NotificationService;
    }

    public async Task StartGameAsync(Guid roomId)
    {
        var user = _userContext.CurrentUser!;

        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new NotFoundException("Room not found.");

        if (room.Status != RoomStatus.Open)
            throw new BadRequestException("Room is not open.");

        var roomUser = room.RoomUsers.FirstOrDefault(ru => ru.UserId == user.Id);
        if (roomUser == null || !roomUser.IsOwner)
            throw new BadRequestException("Only the room owner can start the game.");

        if (room.RoomUsers.Count < 2)
            throw new BadRequestException("At least 2 players are required to start the game.");

        //room.Status = RoomStatus.Closed;
        await _roomRepository.UpdateAsync(room);

        if (room.GameMap == null) throw new AppException("Map data is missing.");

        var gameState = _gameStateManager.InitializeGame(roomId, room.GameMap, room.RoomUsers);

        await _NotificationService.SendEventAsync(roomId.ToString(), "GameStarted", new
        {
            roomId = room.Id.ToString(),
            map = gameState.MapLayout,
            players = gameState.Players.Values
        });

        await _NotificationService.SendEventAsync("SearchRoom", "RoomStatusChanged", new { roomId = room.Id });
    }
}
