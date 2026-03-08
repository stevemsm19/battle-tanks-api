namespace BattleTanks.Domain.Exceptions;

public class InvalidRoomNameException : DomainException
{
    public InvalidRoomNameException() : base("Room name cannot be empty.")
    {
    }
}
