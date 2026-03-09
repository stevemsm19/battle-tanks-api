using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Exceptions;

namespace BattleTanks.Domain.Entities.Users;

public class User
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();

    public User() { }

    private User(string email, string nickname, string externalId)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new EmptyEmailException();
        
        if (string.IsNullOrWhiteSpace(nickname))
            throw new EmptyNicknameException();

        if (string.IsNullOrWhiteSpace(externalId))
            throw new EmptyExternalIdException();

        Id = Guid.NewGuid();
        Email = email;
        Nickname = nickname;
        ExternalId = externalId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static User Create(string email, string nickname, string externalId)
    {
        return new User(email, nickname, externalId);
    }

    public void Update(string nickname, string email, string? avatarUrl = null)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new EmptyNicknameException();

        if (string.IsNullOrWhiteSpace(email))
            throw new EmptyEmailException();

        Nickname = nickname;
        Email = email;
        
        if (avatarUrl != null)
        {
            AvatarUrl = avatarUrl;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
