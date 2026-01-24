namespace BattleTanks.Application.DTOs.Auth;

public class RegisterRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class RegisterResponseDTO
{
    public string Uid { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class LogoutRequestDTO
{
    public string Uid { get; set; } = string.Empty;
}
