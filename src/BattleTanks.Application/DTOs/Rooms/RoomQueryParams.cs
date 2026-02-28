using System.ComponentModel.DataAnnotations;

namespace BattleTanks.Application.DTOs.Rooms;

public record RoomQueryParams(
    [Range(1, 100)] int Page = 1,
    [Range(1, 100)] int Size = 10,
    string? Status = null
);
