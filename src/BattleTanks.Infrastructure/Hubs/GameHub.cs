using BattleTanks.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BattleTanks.Infrastructure.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly IConnectedPlayerService _connectedPlayerService;

    public GameHub(IConnectedPlayerService connectedPlayerService)
    {
        _connectedPlayerService = connectedPlayerService;
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
}
