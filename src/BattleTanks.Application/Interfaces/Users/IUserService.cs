using BattleTanks.Application.DTOs.Users;

namespace BattleTanks.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> GetUserAsync(string firebaseUid);
    Task<UserResponseDTO> UpdateUserAsync(string firebaseUid, UpdateUserRequestDTO request, string currentUserFirebaseUid);
    Task DeleteUserAsync(string firebaseUid, string currentUserFirebaseUid);
}
