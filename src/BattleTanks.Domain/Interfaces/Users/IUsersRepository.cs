using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Domain.Interfaces.Users;

public interface IUsersRepository
{
    Task<UserFirebaseInfo> GetUserAsync(string uid);
    Task UpdateUserAsync(string uid, string displayName);
    Task DeleteUserAsync(string uid);
}
