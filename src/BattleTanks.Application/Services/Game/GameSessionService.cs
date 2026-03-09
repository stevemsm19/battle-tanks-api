using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Exceptions;
using BattleTanks.Application.Interfaces.Game;
using BattleTanks.Domain.Entities.Game;
using BattleTanks.Domain.Interfaces.Rooms;

namespace BattleTanks.Application.Services.Game;

public class GameSessionService : IGameSessionService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserContext _userContext;
    private readonly IGameStateManager _gameStateManager;
    private readonly INotificationService _notificationService;

    private const string GameStartedEvent = "GameStarted";
    private const string RoomStatusChangedEvent = "RoomStatusChanged";
    private const string SearchRoomGroup = "SearchRoom";

    public GameSessionService(
        IRoomRepository roomRepository, 
        IUserContext userContext, 
        IGameStateManager gameStateManager, 
        INotificationService notificationService)
    {
        _roomRepository = roomRepository;
        _userContext = userContext;
        _gameStateManager = gameStateManager;
        _notificationService = notificationService;
    }

    public async Task StartGameAsync(Guid roomId)
    {
        var currentUserId = _userContext.CurrentUser!.Id;

        var room = await _roomRepository.GetByIdAsync(roomId) 
            ?? throw new NotFoundException("Room not found.");

        room.Start(currentUserId);

        await _roomRepository.UpdateAsync(room);

        var gameState = _gameStateManager.InitializeGame(roomId, room.GameMap, room.RoomUsers);

        await Task.WhenAll(
            NotifyPlayers(roomId, gameState),
            NotifyLobbyUpdate(room.Id)
        );
    }

    private async Task NotifyPlayers(Guid roomId, GameState gameState)
    {
        await _notificationService.SendEventAsync(roomId.ToString(), GameStartedEvent, new
        {
            roomId = roomId.ToString(),
            map = gameState.MapLayout,
            players = gameState.Players.Values
        });
    }

    private async Task NotifyLobbyUpdate(Guid roomId)
    {
        await _notificationService.SendEventAsync(SearchRoomGroup, RoomStatusChangedEvent, new { roomId });
    }
}