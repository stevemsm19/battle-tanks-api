namespace BattleTanks.Domain.Exceptions;

public class EmailAlreadyRegisteredException : DomainException
{
    public EmailAlreadyRegisteredException(string email) 
        : base($"Email '{email}' is already registered.")
    {
    }
}
