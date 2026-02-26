namespace BattleTanks.Domain.Entities.Users;

using BattleTanks.Domain.Entities.Rooms;

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
}
