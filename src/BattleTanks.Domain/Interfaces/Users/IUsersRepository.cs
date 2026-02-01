namespace BattleTanks.Domain.Interfaces.Users;

public interface IUsersRepository
{
    Task UpdateUserAsync(string uid, string displayName, string email);
    Task DeleteUserAsync(string uid);
}
