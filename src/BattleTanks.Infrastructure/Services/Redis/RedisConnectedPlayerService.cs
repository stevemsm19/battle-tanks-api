using BattleTanks.Application.Common.Interfaces;
using StackExchange.Redis;

namespace BattleTanks.Infrastructure.Services.Redis;

public class RedisConnectedPlayerService : IConnectedPlayerService
{
    private readonly IDatabase _db;

    private static string PlayerKey(string connectionId) => $"connected:{connectionId}";
    private static string RoomKey(string roomId) => $"room:{roomId}:players";
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);

    public RedisConnectedPlayerService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task AddPlayerAsync(string connectionId, string userId)
    {
        var key = PlayerKey(connectionId);
        await _db.HashSetAsync(key, [new HashEntry("userId", userId)]);
        await _db.KeyExpireAsync(key, Ttl);
    }

    public async Task SetPlayerRoomAsync(string connectionId, string roomId)
    {
        var playerKey = PlayerKey(connectionId);

        var previousRoom = await _db.HashGetAsync(playerKey, "roomId");
        if (previousRoom.HasValue && previousRoom != roomId)
            await _db.SetRemoveAsync(RoomKey(previousRoom!), connectionId);

        await _db.HashSetAsync(playerKey, [new HashEntry("roomId", roomId)]);
        await _db.KeyExpireAsync(playerKey, Ttl);

        var roomKey = RoomKey(roomId);
        await _db.SetAddAsync(roomKey, connectionId);
        await _db.KeyExpireAsync(roomKey, Ttl);
    }

    public async Task RemovePlayerAsync(string connectionId)
    {
        var playerKey = PlayerKey(connectionId);
        var roomId = await _db.HashGetAsync(playerKey, "roomId");
        if (roomId.HasValue)
            await _db.SetRemoveAsync(RoomKey(roomId!), connectionId);
        await _db.KeyDeleteAsync(playerKey);
    }

    public async Task<string?> GetPlayerRoomAsync(string connectionId)
    {
        var roomId = await _db.HashGetAsync(PlayerKey(connectionId), "roomId");
        return roomId.HasValue ? roomId.ToString() : null;
    }
}
