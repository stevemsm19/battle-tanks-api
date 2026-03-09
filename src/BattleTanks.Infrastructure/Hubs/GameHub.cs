using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Interfaces.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BattleTanks.Infrastructure.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly IConnectedPlayerService _connectedPlayerService;
    private readonly IGameStateManager _gameStateManager;
    
    private const string SearchRoomGroup = "SearchRoom";

    public GameHub(IConnectedPlayerService connectedPlayerService, IGameStateManager gameStateManager)
    {
        _connectedPlayerService = connectedPlayerService;
        _gameStateManager = gameStateManager;
    }

    private string UserId => Context.UserIdentifier 
        ?? throw new HubException("User not authenticated");

    public override async Task OnConnectedAsync()
    {
        await _connectedPlayerService.AddPlayerAsync(Context.ConnectionId, UserId);

        var roomId = await _connectedPlayerService.GetPlayerRoomAsync(UserId);
        if (!string.IsNullOrEmpty(roomId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await SyncStateToCaller(roomId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
{
    var roomId = await _connectedPlayerService.GetPlayerRoomAsync(UserId);
    
    if (!string.IsNullOrEmpty(roomId) && TryParseRoomId(roomId, out var roomGuid))
    {
        var gameState = _gameStateManager.GetGameState(roomGuid);
        gameState?.Players.TryRemove(UserId, out _); 

        await Clients.Group(roomId).SendAsync("PlayerLeft", UserId);
    }

    await _connectedPlayerService.RemovePlayerAsync(Context.ConnectionId);
    await base.OnDisconnectedAsync(exception);
}

    public async Task JoinGame(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await _connectedPlayerService.SetPlayerRoomAsync(Context.ConnectionId, roomId);
        await SyncStateToCaller(roomId);
    }

    public async Task LeaveGame(string roomId)
{
    if (TryParseRoomId(roomId, out var roomGuid))
    {
        var gameState = _gameStateManager.GetGameState(roomGuid);
        gameState?.Players.TryRemove(UserId, out _);
    }

    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    await _connectedPlayerService.ClearPlayerSessionAsync(UserId);
    
    await Clients.Group(roomId).SendAsync("PlayerLeft", UserId);
}

    public Task JoinSearchRoomGroup() => Groups.AddToGroupAsync(Context.ConnectionId, SearchRoomGroup);
    public Task LeaveSearchRoomGroup() => Groups.RemoveFromGroupAsync(Context.ConnectionId, SearchRoomGroup);

    public async Task SendMessage(string roomId, string user, string message)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMessage", new { sender = user, text = message });
    }

    public Task ChangeDirection(string roomId, string direction)
    {
        if (TryParseRoomId(roomId, out var roomGuid))
            _gameStateManager.SetPlayerDirection(roomGuid, UserId, direction);

        return Task.CompletedTask;
    }

    public async Task Shoot(string roomId)
    {
        if (TryParseRoomId(roomId, out var roomGuid))
        {
            var bullet = _gameStateManager.SpawnBullet(roomGuid, UserId);
            if (bullet != null)
            {
                await Clients.Group(roomId).SendAsync("BulletSpawned", new
                {
                    id = bullet.Id,
                    x = bullet.X,
                    y = bullet.Y,
                    direction = bullet.Direction
                });
            }
        }
    }

    private async Task SyncStateToCaller(string roomId)
    {
        if (TryParseRoomId(roomId, out var roomGuid))
        {
            var gameState = _gameStateManager.GetGameState(roomGuid);
            if (gameState != null)
                await Clients.Caller.SendAsync("SyncGameState", gameState);
        }
    }

    private bool TryParseRoomId(string roomId, out Guid guid) => Guid.TryParse(roomId, out guid);
}