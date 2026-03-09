using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Interfaces.Users;
using StackExchange.Redis;

namespace BattleTanks.Infrastructure.Services.Redis;

public class LeaderboardService : ILeaderboardService
{
    private readonly IDatabase _db;
    private readonly IUserRepository _userRepository;
    private const string ScoresKey = "leaderboard:scores";

    public LeaderboardService(IConnectionMultiplexer redis, IUserRepository userRepository)
    {
        _db = redis.GetDatabase();
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<TopPlayerDTO>> GetTop10Async()
    {
        if (!await _db.KeyExistsAsync(ScoresKey))
            await RefreshCacheAsync();

        var entries = await _db.SortedSetRangeByRankWithScoresAsync(ScoresKey, 0, 9, Order.Descending);

        return entries.Select((e, i) => new TopPlayerDTO(i + 1, e.Element!, (int)e.Score));
    }

    public async Task UpdateScoreAsync(string nickname, int score)
    {
        await _db.SortedSetAddAsync(ScoresKey, nickname, score);
        await _db.SortedSetRemoveRangeByRankAsync(ScoresKey, 0, -101);
    }

    private async Task RefreshCacheAsync()
    {
        var top100 = await _userRepository.GetTop100ByScoreAsync();
        
        var entries = top100
            .Select(u => new SortedSetEntry(u.Nickname, u.Score))
            .ToArray();

        if (entries.Length > 0)
        {
            await _db.SortedSetAddAsync(ScoresKey, entries);
            await _db.KeyExpireAsync(ScoresKey, TimeSpan.FromHours(24)); 
        }
    }
}