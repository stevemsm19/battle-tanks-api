using BattleTanks.Application.Common.Interfaces;
using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Infrastructure.Services;

public class UserContext : IUserContext
{
    public User? CurrentUser { get; set; }
}
