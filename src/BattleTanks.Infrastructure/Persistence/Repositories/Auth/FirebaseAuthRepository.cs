using BattleTanks.Domain.Interfaces.Auth;
using FirebaseAdmin.Auth;

namespace BattleTanks.Infrastructure.Persistence.Repositories.Auth;

public class FirebaseAuthRepository : IAuthRepository
{
    public async Task<string> CreateUserAsync(string email, string password, string displayName)
    {
        var args = new UserRecordArgs
        {
            Email = email,
            Password = password,
            DisplayName = displayName,
            EmailVerified = false
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        return userRecord.Uid;
    }

    public async Task RevokeRefreshTokensAsync(string uid)
    {
        await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(uid);
    }
}
