namespace BattleTanks.Domain.Exceptions;

public class RoomFullException : DomainException
{
    public RoomFullException() : base("Room is full")
    {
    }
}
