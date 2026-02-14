namespace BattleTanks.Domain.Interfaces;

public interface IIdentityProvider
{
    Task<string> CreateUserAsync(string email, string password, string nickname);
    Task UpdateUserAsync(string uid, string? nickname, string? email);
    Task DeleteUserAsync(string uid);
}
