namespace BattleTanks.Domain.Exceptions;

public class RoomClosedException : DomainException
{
    public RoomClosedException() : base("Room is closed or cancelled")
    {
    }
}
