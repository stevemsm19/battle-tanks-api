namespace BattleTanks.Application.Common.Interfaces;

public interface IConnectedPlayerService
{
    Task AddPlayerAsync(string connectionId, string userId);
    Task SetPlayerRoomAsync(string connectionId, string roomId);
    Task RemovePlayerAsync(string connectionId);
    Task<string?> GetPlayerRoomAsync(string connectionId);
}
