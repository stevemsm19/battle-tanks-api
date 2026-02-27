using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BattleTanks.Infrastructure.Persistence.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly BattleTanksDbContext _dbContext;

    public UserRepository(BattleTanksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByFirebaseUidAsync(string firebaseUid)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<bool> ExistsByNicknameAsync(string nickname)
    {
        return await _dbContext.Users.AnyAsync(u => u.Nickname.ToLower() == nickname.ToLower());
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public void Delete(User user)
    {
        _dbContext.Users.Remove(user);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetTop100ByScoreAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(u => u.Score)
            .Take(100)
            .ToListAsync();
    }
}
