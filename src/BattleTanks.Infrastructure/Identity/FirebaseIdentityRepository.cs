using BattleTanks.Domain.Interfaces;
using FirebaseAdmin.Auth;

namespace BattleTanks.Infrastructure.Persistence.Repositories;

public class FirebaseIdentityRepository : IIdentityProvider
{
    public async Task<string> CreateUserAsync(string email, string password, string nickname)
    {
        var args = new UserRecordArgs
        {
            Email = email,
            Password = password,
            DisplayName = nickname,
            EmailVerified = false
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        return userRecord.Uid;
    }

    public async Task UpdateUserAsync(string uid, string? nickname, string? email, string? password)
    {
        var args = new UserRecordArgs
        {
            Uid = uid,
        };

        if (!string.IsNullOrWhiteSpace(nickname)) 
            args.DisplayName = nickname;

        if (!string.IsNullOrWhiteSpace(email)) 
            args.Email = email;

        if (!string.IsNullOrWhiteSpace(password))
            args.Password = password;

        await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
    }

    public async Task DeleteUserAsync(string uid)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
    }
}
