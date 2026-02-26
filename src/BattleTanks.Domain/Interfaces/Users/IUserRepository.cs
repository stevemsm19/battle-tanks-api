using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Domain.Interfaces.Users;

public interface IUserRepository
{
    Task<User?> GetByFirebaseUidAsync(string firebaseUid);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNicknameAsync(string nickname);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task SaveChangesAsync();
    Task<IEnumerable<User>> GetTop100ByScoreAsync();
}
