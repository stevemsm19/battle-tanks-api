using BattleTanks.Application.DTOs.Users;

namespace BattleTanks.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> GetUserAsync(string currentUserFirebaseUid);
    Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO request, string currentUserFirebaseUid);
    Task DeleteUserAsync(string currentUserFirebaseUid);
}
