namespace BattleTanks.Domain.Exceptions;

public class InsufficientPlayersException : DomainException
{
    public InsufficientPlayersException() : base("Not enough players to start the game.")
    {
    }
}
