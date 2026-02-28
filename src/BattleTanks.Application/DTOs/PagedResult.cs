namespace BattleTanks.Application.DTOs;

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int Size
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / Size);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}