using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;

namespace BattleTanks.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IUserRepository _userRepository;

    public UserService(IIdentityProvider identityProvider, IUserRepository userRepository)
    {
        _identityProvider = identityProvider;
        _userRepository = userRepository;
    }

    public async Task<UserResponseDTO> GetUserAsync(string currentUserFirebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(currentUserFirebaseUid);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new UserResponseDTO
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Email = user.Email,
            Nickname = user.Nickname,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO request, string currentUserFirebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(currentUserFirebaseUid);
        
        if (user == null)
        {
            throw new Exception("User not found");
        }

        await _identityProvider.UpdateUserAsync(currentUserFirebaseUid, request.Nickname, request.Email, request.Password);

        if (!string.IsNullOrWhiteSpace(request.Nickname))
        {
            user.Nickname = request.Nickname;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            user.Email = request.Email;
        }

        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new UserResponseDTO
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Email = user.Email,
            Nickname = user.Nickname,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task DeleteUserAsync(string currentUserFirebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(currentUserFirebaseUid);
        
        if (user == null)
        {
            throw new Exception("User not found");
        }

        await _identityProvider.DeleteUserAsync(currentUserFirebaseUid);

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }
}

