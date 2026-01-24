using BattleTanks.Application.DTOs.Auth;

namespace BattleTanks.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
    Task LogoutAsync(LogoutRequestDTO request);
}
