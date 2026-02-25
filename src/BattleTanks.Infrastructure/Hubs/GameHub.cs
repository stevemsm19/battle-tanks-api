using Microsoft.AspNetCore.SignalR;

namespace BattleTanks.Infrastructure.Hubs;

public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }
    
    public async Task LeaveGame(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
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
