using System.Threading.Tasks;

namespace BattleTanks.Application.Interfaces;

public interface INotificationService
{
    Task SendEventAsync(string roomId, string eventName, object data);
    Task SendMessageAsync(string roomId, string user, string message);
}
