using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Users;

public record UpdateUserRequestDTO(
    [Required, StringLength(50, MinimumLength = 3)] string Nickname,
    [Required, EmailAddress] string Email,
    string? AvatarUrl
);

public record UserResponseDTO(
    Guid Id,
    string ExternalId,
    string Email,
    string Nickname,
    string AvatarUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
