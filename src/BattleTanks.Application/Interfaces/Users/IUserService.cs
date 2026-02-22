using BattleTanks.Application.DTOs.Users;

namespace BattleTanks.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> GetUserAsync();
    Task<UserResponseDTO> UpdateUserAsync(UpdateUserRequestDTO request);
    Task DeleteUserAsync();
}
