namespace BattleTanks.Domain.Exceptions;

public class EmptyEmailException : DomainException
{
    public EmptyEmailException() : base("Email cannot be empty.")
    {
    }
}
