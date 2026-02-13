using BattleTanks.Application.DTOs.Auth;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IUserRepository _userRepository;

    public AuthService(IIdentityProvider identityProvider, IUserRepository userRepository)
    {
        _identityProvider = identityProvider;
        _userRepository = userRepository;
    }

    public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
    {
        var uid = await _identityProvider.CreateUserAsync(
            request.Email, 
            request.Password, 
            request.Nickname
        );

        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirebaseUid = uid,
                Email = request.Email,
                Nickname = request.Nickname,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            await _identityProvider.DeleteUserAsync(uid);
            throw;
        }

        return new RegisterResponseDTO 
        { 
            Uid = uid, 
            Message = "User registered successfully." 
        };
    }
}
