using System.Diagnostics;
using BattleTanks.Domain.Entities.Rooms;
using BattleTanks.Domain.Interfaces.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BattleTanks.Infrastructure.Persistence.Repositories.Rooms;

public class RoomRepository : IRoomRepository
{
    private readonly BattleTanksDbContext _dbContext;

    public RoomRepository(BattleTanksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        var result = await _dbContext.Rooms
            .Include(r => r.RoomUsers)
            .ThenInclude(ru => ru.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        return result;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        var result = await _dbContext.Rooms
            .AsNoTracking()
            .Include(r => r.RoomUsers)
            .ThenInclude(ru => ru.User)
            .ToListAsync();
        return result;
    }

    public async Task AddAsync(Room room)
    {
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Room room)
    {
        _dbContext.Rooms.Update(room);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Room room)
    {
        _dbContext.Rooms.Remove(room);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Room?> GetByUserIdAsync(Guid userId)
    {
        var result = await _dbContext.Rooms
            .AsNoTracking()
            .Include(r => r.RoomUsers)
            .FirstOrDefaultAsync(r =>
                r.Status == BattleTanks.Domain.Enums.RoomStatus.Open &&
                r.RoomUsers.Any(ru => ru.UserId == userId));
        return result;
    }
}
