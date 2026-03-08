using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Exceptions;

namespace BattleTanks.Domain.Entities.Users;

public class User
{
    public Guid Id { get; set; }
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<RoomUser> RoomUsers { get; set; } = new List<RoomUser>();

    public User() { }

    private User(string email, string nickname, string firebaseUid)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(nickname))
            throw new DomainException("Nickname cannot be empty.");

        if (string.IsNullOrWhiteSpace(firebaseUid))
            throw new DomainException("FirebaseUid cannot be empty.");

        Id = Guid.NewGuid();
        Email = email;
        Nickname = nickname;
        FirebaseUid = firebaseUid;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static User Create(string email, string nickname, string firebaseUid)
    {
        return new User(email, nickname, firebaseUid);
    }

    public void Update(string nickname, string email, string? avatarUrl = null)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new DomainException("Nickname cannot be empty.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");

        Nickname = nickname;
        Email = email;
        
        if (avatarUrl != null)
        {
            AvatarUrl = avatarUrl;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
