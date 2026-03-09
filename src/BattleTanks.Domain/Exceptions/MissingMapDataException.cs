namespace BattleTanks.Domain.Exceptions;

public class MissingMapDataException : DomainException
{
    public MissingMapDataException() : base("Game map data is required to start.")
    {
    }
}
