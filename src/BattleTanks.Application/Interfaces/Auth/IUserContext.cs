using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Application.Interfaces;

public interface IUserContext
{
    User? CurrentUser { get; set; }
}
