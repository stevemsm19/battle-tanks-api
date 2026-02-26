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
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(24);

    public LeaderboardService(IConnectionMultiplexer redis, IUserRepository userRepository)
    {
        _db = redis.GetDatabase();
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<TopPlayerDTO>> GetTop10Async()
    {
        var count = await _db.SortedSetLengthAsync(ScoresKey);
        if (count == 0)
            await SeedFromDatabaseAsync();

        var entries = await _db.SortedSetRangeByRankWithScoresAsync(
            ScoresKey, start: 0, stop: 9, order: Order.Descending);

        return entries.Select((e, i) => new TopPlayerDTO(
            Rank: i + 1,
            Nickname: e.Element!,
            Score: (int)e.Score
        ));
    }

    private async Task SeedFromDatabaseAsync()
    {
        var top100 = await _userRepository.GetTop100ByScoreAsync();

        var entries = top100
            .Select(u => new SortedSetEntry(u.Nickname, u.Score))
            .ToArray();

        if (entries.Length == 0) return;

        await _db.SortedSetAddAsync(ScoresKey, entries);
        await _db.KeyExpireAsync(ScoresKey, Ttl);
    }
}
