namespace BattleTanks.Domain.Exceptions;

public class IdentityException : InfrastructureException
{
    public string Code { get; }

    public IdentityException(string code, string message) : base(message)
    {
        Code = code;
    }
}
