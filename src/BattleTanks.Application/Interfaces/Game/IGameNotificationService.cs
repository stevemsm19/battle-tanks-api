using System.Threading.Tasks;

namespace BattleTanks.Application.Interfaces.Game;

public interface IGameNotificationService
{
    Task SendGameEventAsync(string roomId, string eventName, object data);
    Task SendMessageAsync(string roomId, string user, string message);
}
