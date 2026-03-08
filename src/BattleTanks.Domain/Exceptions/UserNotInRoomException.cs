namespace BattleTanks.Domain.Exceptions;

public class UserNotInRoomException : DomainException
{
    public UserNotInRoomException() : base("User does not belong to this room.")
    {
    }
}
