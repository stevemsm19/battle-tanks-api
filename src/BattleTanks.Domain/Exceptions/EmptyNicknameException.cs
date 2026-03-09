namespace BattleTanks.Domain.Exceptions;

public class EmptyNicknameException : DomainException
{
    public EmptyNicknameException() : base("Nickname cannot be empty.")
    {
    }
}
