using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;

using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Exceptions;

using MapsterMapper;

namespace BattleTanks.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public UserService(IIdentityProvider identityProvider, IUserRepository userRepository, IUserContext userContext, IMapper mapper)
    {
        _identityProvider = identityProvider;
        _userRepository = userRepository;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<UserResponseDTO> GetUserAsync()
    {
        var user = _userContext.CurrentUser!;
        return _mapper.Map<UserResponseDTO>(user);
    }

    public async Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO request)
    {
        var user = _userContext.CurrentUser!;

        await _identityProvider.UpdateUserAsync(user.ExternalId, request.Nickname, request.Email);

        user.Update(request.Nickname, request.Email, request.AvatarUrl);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserResponseDTO>(user);
    }

    public async Task DeleteUserAsync()
    {
        var user = _userContext.CurrentUser!;

        await _identityProvider.DeleteUserAsync(user.ExternalId);

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }
}

