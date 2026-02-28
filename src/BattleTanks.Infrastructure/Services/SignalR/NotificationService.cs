using BattleTanks.Infrastructure.Hubs;
using BattleTanks.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BattleTanks.Infrastructure.Services.SignalR;

public class NotificationService : INotificationService
{
    private readonly IHubContext<GameHub> _hubContext;

    public NotificationService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendEventAsync(string roomId, string eventName, object data)
    {
        await _hubContext.Clients.Group(roomId).SendAsync(eventName, data);
    }

    public async Task SendMessageAsync(string roomId, string user, string message)
    {
        await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);
    }
}
