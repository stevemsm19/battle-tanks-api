using BattleTanks.Domain.Exceptions;
using BattleTanks.Domain.Interfaces;
using FirebaseAdmin.Auth;

namespace BattleTanks.Infrastructure.Persistence.Repositories;

public class FirebaseIdentityRepository : IIdentityProvider
{
    public async Task<string> CreateUserAsync(string email, string password, string nickname)
    {
        try
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
        catch (FirebaseAuthException ex)
        {   
            throw new IdentityException(ex.AuthErrorCode?.ToString() ?? "Unknown", ex.Message);
        }
    }

    public async Task UpdateUserAsync(string uid, string? nickname, string? email, string? password)
    {
        try
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
        catch (FirebaseAuthException ex)
        {
            throw new IdentityException(ex.AuthErrorCode?.ToString() ?? "Unknown", ex.Message);
        }
    }

    public async Task DeleteUserAsync(string uid)
    {
        try
        {
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
        }
        catch (FirebaseAuthException ex)
        {
            throw new IdentityException(ex.AuthErrorCode?.ToString() ?? "Unknown", ex.Message);
        }
    }
}

