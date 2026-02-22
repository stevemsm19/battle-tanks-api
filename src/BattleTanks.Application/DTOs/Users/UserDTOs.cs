using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Users;

public class UpdateUserRequestDTO
{
    [Required(ErrorMessage = "Nickname is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nickname must be between 3 and 50 characters.")]
    public string Nickname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
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
