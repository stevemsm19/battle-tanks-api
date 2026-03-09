using BattleTanks.Application.Interfaces;
using StackExchange.Redis;

namespace BattleTanks.Infrastructure.Services.Redis;

public class RedisConnectedPlayerService : IConnectedPlayerService
{
    private readonly IDatabase _db;
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(2);

    private static string UserKey(string id) => $"user:session:{id}";
    private static string ConnKey(string id) => $"conn:to:user:{id}";
    private static string RoomKey(string id) => $"room:{id}:players";

    public RedisConnectedPlayerService(IConnectionMultiplexer redis) => _db = redis.GetDatabase();

    public async Task AddPlayerAsync(string connectionId, string userId)
    {
        await Task.WhenAll(
            _db.StringSetAsync(ConnKey(connectionId), userId, Ttl),
            _db.HashSetAsync(UserKey(userId), "lastConnectionId", connectionId)
        );
        await _db.KeyExpireAsync(UserKey(userId), Ttl);
    }

    public async Task SetPlayerRoomAsync(string connectionId, string roomId)
    {
        var userId = await _db.StringGetAsync(ConnKey(connectionId));
        if (!userId.HasValue) return;

        await _db.HashSetAsync(UserKey(userId!), "roomId", roomId);
        await _db.SetAddAsync(RoomKey(roomId), userId!);
    }

    public async Task RemovePlayerAsync(string connectionId)
    {
        await _db.KeyDeleteAsync(ConnKey(connectionId));
    }

    public async Task<string?> GetPlayerRoomAsync(string userId)
    {
        var roomId = await _db.HashGetAsync(UserKey(userId), "roomId");
        return roomId;
    }

    public async Task ClearPlayerSessionAsync(string userId)
    {
        var roomId = await _db.HashGetAsync(UserKey(userId), "roomId");

        if (roomId.HasValue)
        {
            await _db.SetRemoveAsync(RoomKey(roomId!), userId);
        }

        await _db.KeyDeleteAsync(UserKey(userId));
    }
}