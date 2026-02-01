using BattleTanks.Application.DTOs.Auth;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Interfaces.Auth;
using BattleTanks.Domain.Interfaces.Users;

namespace BattleTanks.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserManagementService _userManagementService;

    public AuthService(IAuthRepository authRepository, IUserManagementService userManagementService)
    {
        _authRepository = authRepository;
        _userManagementService = userManagementService;
    }

    public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        var uid = await _authRepository.CreateUserAsync(
            request.Email, 
            request.Password, 
            request.DisplayName
        );

        await _userManagementService.CreateUserInDatabaseAsync(
            uid,
            request.Email,
            request.DisplayName
        );

        return new RegisterResponseDTO 
        { 

            Uid = uid, 
            Message = "User registered successfully." 
        };
    }
}
