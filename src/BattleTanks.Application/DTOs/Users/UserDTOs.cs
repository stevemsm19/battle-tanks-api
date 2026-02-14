using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Users;

public class UpdateUserRequestDTO
{
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
