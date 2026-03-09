namespace BattleTanks.Domain.Exceptions;

public class NotRoomOwnerException : DomainException
{
    public NotRoomOwnerException() : base("Only the owner can perform this action.")
    {
    }
}
