using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces.Users;
using FirebaseAdmin.Auth;

namespace BattleTanks.Infrastructure.Persistence.Repositories.Users;

public class FirebaseUsersRepository : IUsersRepository
{
    public async Task<UserFirebaseInfo> GetUserAsync(string uid)
    {
        var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        
        return new UserFirebaseInfo
        {
            Uid = userRecord.Uid,
            Email = userRecord.Email,
            DisplayName = userRecord.DisplayName
        };
    }

    public async Task UpdateUserAsync(string uid, string displayName)
    {
        var args = new UserRecordArgs
        {
            Uid = uid,
            DisplayName = displayName
        };

        await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
    }

    public async Task DeleteUserAsync(string uid)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
    }
}
