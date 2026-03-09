using BattleTanks.Application.DTOs.Auth;
using BattleTanks.Application.Exceptions;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Exceptions;
using MapsterMapper;

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
        var nicknameExists = await _userRepository.ExistsByNicknameAsync(request.Nickname);
        if (nicknameExists)
            throw new NicknameAlreadyTakenException(request.Nickname);
        
        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email);
        if (emailExists)
            throw new EmailAlreadyRegisteredException(request.Email);

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
                throw new EmailAlreadyRegisteredException(request.Email);

            throw new ExternalServiceException("Identity service failed.");
        }

        try
        {
            var user = User.Create(request.Email, request.Nickname, uid);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            await _identityProvider.DeleteUserAsync(uid);
            throw;
        }

        return new RegisterResponseDTO(uid, "User registered successfully.");
    }
}

