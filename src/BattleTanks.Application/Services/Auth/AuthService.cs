using BattleTanks.Application.DTOs.Auth;
using BattleTanks.Application.Exceptions;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Exceptions;

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
        string uid;

        try
        {
            uid = await _identityProvider.CreateUserAsync(
                request.Email, 
                request.Password, 
                request.Nickname
            );
        }
        catch (IdentityException ex)
        {
            if (ex.Code == "EmailAlreadyExists")
                throw new BadRequestException("Email already registered.");

            throw new ExternalServiceException("Identity service failed.");
        }

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

