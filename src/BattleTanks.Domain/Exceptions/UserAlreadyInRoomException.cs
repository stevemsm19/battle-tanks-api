namespace BattleTanks.Domain.Exceptions;

public class UserAlreadyInRoomException : DomainException
{
    public UserAlreadyInRoomException() : base("User is already in this room.")
    {
    }

    public UserAlreadyInRoomException(Guid roomId) : base($"User belongs to another room: {roomId}")
    {
    }
}
