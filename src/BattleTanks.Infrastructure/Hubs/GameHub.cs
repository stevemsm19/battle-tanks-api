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

    public GameHub(IConnectedPlayerService connectedPlayerService, IGameStateManager gameStateManager)
    {
        _connectedPlayerService = connectedPlayerService;
        _gameStateManager = gameStateManager;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        await _connectedPlayerService.AddPlayerAsync(Context.ConnectionId, userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _connectedPlayerService.RemovePlayerAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await _connectedPlayerService.SetPlayerRoomAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveGame(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await _connectedPlayerService.RemovePlayerAsync(Context.ConnectionId);

        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        await _connectedPlayerService.AddPlayerAsync(Context.ConnectionId, userId);
    }

    public async Task JoinSearchRoomGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "SearchRoom");
    }

    public async Task LeaveSearchRoomGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SearchRoom");
    }

    public async Task SendMessage(string roomId, string user, string message)
    {
        await Clients.Group(roomId).SendAsync("ReceiveMessage", new { sender = user, text = message });
    }

    public Task ChangeDirection(string roomId, string direction)
    {
        var firebaseUid = Context.UserIdentifier ?? Context.ConnectionId;
        if (Guid.TryParse(roomId, out var roomGuid))
            _gameStateManager.SetPlayerDirection(roomGuid, firebaseUid, direction);

        return Task.CompletedTask;
    }

    public Task Shoot(string roomId)
    {
        var firebaseUid = Context.UserIdentifier ?? Context.ConnectionId;
        if (Guid.TryParse(roomId, out var roomGuid))
            _gameStateManager.SpawnBullet(roomGuid, firebaseUid);

        return Task.CompletedTask;
    }
}
