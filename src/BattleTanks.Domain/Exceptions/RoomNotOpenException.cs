namespace BattleTanks.Domain.Exceptions;

public class RoomNotOpenException : DomainException
{
    public RoomNotOpenException() : base("The room is not in an open state.")
    {
    }
}