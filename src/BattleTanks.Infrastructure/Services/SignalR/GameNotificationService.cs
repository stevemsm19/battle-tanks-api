using BattleTanks.Infrastructure.Hubs;
using BattleTanks.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BattleTanks.Infrastructure.Services.SignalR;

public class GameNotificationService : IGameNotificationService
{
    private readonly IHubContext<GameHub> _hubContext;

    public GameNotificationService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendGameEventAsync(string roomId, string eventName, object data)
    {
        await _hubContext.Clients.Group(roomId).SendAsync(eventName, data);
    }

    public async Task SendMessageAsync(string roomId, string user, string message)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);
    }
}
