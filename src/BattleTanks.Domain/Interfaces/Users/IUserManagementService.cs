namespace BattleTanks.Domain.Interfaces.Users;

public interface IUserManagementService
{
    Task<UserCreatedDto> CreateUserInDatabaseAsync(string firebaseUid, string email, string displayName);
}

public class UserCreatedDto
{
    public required Guid Id { get; init; }
    public required string FirebaseUid { get; init; }
    public required string Email { get; init; }
    public required string DisplayName { get; init; }
    public required DateTime CreatedAt { get; init; }
}
