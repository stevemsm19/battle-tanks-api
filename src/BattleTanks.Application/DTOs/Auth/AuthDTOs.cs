using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Auth;

public class RegisterRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Nickname { get; set; } = string.Empty;
}

public class RegisterResponseDTO
{
    public string Uid { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}