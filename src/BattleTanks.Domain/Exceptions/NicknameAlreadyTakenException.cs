namespace BattleTanks.Domain.Exceptions;

public class NicknameAlreadyTakenException : DomainException
{
    public NicknameAlreadyTakenException(string nickname) 
        : base($"Nickname '{nickname}' is already taken.")
    {
    }
}
