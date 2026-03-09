namespace BattleTanks.Domain.Exceptions;

public class EmptyExternalIdException : DomainException
{
    public EmptyExternalIdException() : base("External identity ID cannot be empty.")
    {
    }
}
