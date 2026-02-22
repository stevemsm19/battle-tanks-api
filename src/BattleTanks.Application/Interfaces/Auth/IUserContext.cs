using BattleTanks.Domain.Entities.Users;

namespace BattleTanks.Application.Common.Interfaces;

public interface IUserContext
{
    User? CurrentUser { get; set; }
}
