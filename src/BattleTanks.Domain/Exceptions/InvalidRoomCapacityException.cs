namespace BattleTanks.Domain.Exceptions;

public class InvalidRoomCapacityException : DomainException
{
    public InvalidRoomCapacityException() : base("Room capacity must be between 2 and 4 players.")
    {
    }
}
