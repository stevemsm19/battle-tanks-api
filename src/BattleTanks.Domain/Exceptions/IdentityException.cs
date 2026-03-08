namespace BattleTanks.Domain.Exceptions;

public class IdentityException : Exception
{
    public string Code { get; }

    public IdentityException(string code, string message) : base(message)
    {
        Code = code;
    }
}
