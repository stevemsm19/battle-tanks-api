using BattleTanks.Application.DTOs.Users;

namespace BattleTanks.Application.Interfaces;

public interface ILeaderboardService
{
    Task<IEnumerable<TopPlayerDTO>> GetTop10Async();
    Task UpdateScoreAsync(string nickname, int score);
}
