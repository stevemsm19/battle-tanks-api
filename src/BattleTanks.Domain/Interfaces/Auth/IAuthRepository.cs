namespace BattleTanks.Domain.Interfaces.Auth;

public interface IAuthRepository
{
    Task<string> CreateUserAsync(string email, string password, string displayName);
}
