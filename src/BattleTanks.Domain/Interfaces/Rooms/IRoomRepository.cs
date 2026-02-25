using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Domain.Interfaces.Rooms;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Room> Items, int TotalCount)> GetAllAsync(int page, int size);
    Task AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(Room room);
    Task<Room?> GetByUserIdAsync(Guid userId);
}
