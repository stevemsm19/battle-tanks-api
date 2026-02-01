using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces.Users;
using FirebaseAdmin.Auth;

namespace BattleTanks.Infrastructure.Persistence.Repositories.Users;

public class FirebaseUsersRepository : IUsersRepository
{
    public async Task UpdateUserAsync(string uid, string displayName, string email)
    {
        var args = new UserRecordArgs
        {
            Uid = uid,
        };

        if (!string.IsNullOrWhiteSpace(displayName)) 
            args.DisplayName = displayName;

        if (!string.IsNullOrWhiteSpace(email)) 
            args.Email = email;

        await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
    }

    public async Task DeleteUserAsync(string uid)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
    }
}
