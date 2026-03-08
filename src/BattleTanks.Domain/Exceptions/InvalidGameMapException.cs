namespace BattleTanks.Domain.Exceptions;

public class InvalidGameMapException : DomainException
{
    public InvalidGameMapException() : base("Invalid map.")
    {
    }
}
