using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using BattleTanks.Domain.Entities.Users;
using BattleTanks.Domain.Interfaces.Users;

namespace BattleTanks.Application.Services.Users;

public class UserService : IUserService, IUserManagementService
{
    private readonly IUsersRepository _firebaseRepository;
    private readonly IUserRepository _userRepository;

    public UserService(IUsersRepository firebaseRepository, IUserRepository userRepository)
    {
        _firebaseRepository = firebaseRepository;
        _userRepository = userRepository;
    }

    public async Task<UserCreatedDto> CreateUserInDatabaseAsync(string firebaseUid, string email, string displayName)
    {
        var existingUser = await _userRepository.GetByFirebaseUidAsync(firebaseUid);
        if (existingUser != null)
        {
            throw new Exception("User already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirebaseUid = firebaseUid,
            Email = email,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserCreatedDto
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserResponseDTO> GetUserAsync(string firebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(firebaseUid);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new UserResponseDTO
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserResponseDTO> UpdateUserAsync(string firebaseUid, UpdateUserRequestDTO request, string currentUserFirebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(firebaseUid);
        
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (firebaseUid != currentUserFirebaseUid)
        {
            throw new UnauthorizedAccessException("You can only update your own profile");
        }

        await _firebaseRepository.UpdateUserAsync(firebaseUid, request.DisplayName);

        user.DisplayName = request.DisplayName;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new UserResponseDTO
        {
            Id = user.Id,
            FirebaseUid = user.FirebaseUid,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task DeleteUserAsync(string firebaseUid, string currentUserFirebaseUid)
    {
        var user = await _userRepository.GetByFirebaseUidAsync(firebaseUid);
        
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (firebaseUid != currentUserFirebaseUid)
        {
            throw new UnauthorizedAccessException("You can only delete your own account");
        }

        await _firebaseRepository.DeleteUserAsync(firebaseUid);

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }
}

