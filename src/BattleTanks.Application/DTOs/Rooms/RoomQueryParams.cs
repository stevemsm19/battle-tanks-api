using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Rooms;

public class RoomQueryParams
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Size must be between 1 and 100.")]
    public int Size { get; set; } = 10;
}
