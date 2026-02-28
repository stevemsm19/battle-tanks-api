using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Auth;

public record RegisterRequestDTO
(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required] string Nickname
);

public record RegisterResponseDTO(string Uid, string Message);