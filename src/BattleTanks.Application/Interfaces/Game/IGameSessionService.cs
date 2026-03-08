namespace BattleTanks.Application.Interfaces.Game;

public interface IGameSessionService
{
    Task StartGameAsync(Guid roomId);
}
