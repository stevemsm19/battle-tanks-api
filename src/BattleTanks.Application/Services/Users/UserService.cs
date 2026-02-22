using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Application.Exceptions;
using BattleTanks.Application.Common.Interfaces;

namespace BattleTanks.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public UserService(IIdentityProvider identityProvider, IUserRepository userRepository, IUserContext userContext)
    {
        _identityProvider = identityProvider;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public Task<UserResponseDTO> GetUserAsync()
    {
        var user = _userContext.CurrentUser!;

        return Task.FromResult(new UserResponseDTO
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Email = user.Email,
            Nickname = user.Nickname,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }

    public async Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO request)
    {
        var user = _userContext.CurrentUser!;

        await _identityProvider.UpdateUserAsync(user.FirebaseUid, request.Nickname, request.Email);

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

    public async Task DeleteUserAsync()
    {
        var user = _userContext.CurrentUser!;

        await _identityProvider.DeleteUserAsync(user.FirebaseUid);

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }
}

