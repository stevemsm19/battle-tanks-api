using System.ComponentModel.DataAnnotations;
using BattleTanks.Domain.Entities.Rooms;

namespace BattleTanks.Application.DTOs.Rooms;

public class RoomDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public int CurrentPlayers { get; set; }
    public string Status { get; set; } = string.Empty;
    
    public static RoomDTO FromEntity(Room room)
    {
        return new RoomDTO
        {
            Id = room.Id,
            Name = room.Name,
            MaxPlayers = room.MaxPlayers,
            CurrentPlayers = room.RoomUsers.Count,
            Status = room.Status.ToString()
        };
    }
}

public class CreateRoomDTO
{
    [Required(ErrorMessage = "Room name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Room name must be between 3 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(2, 4, ErrorMessage = "Max players must be between 2 and 4.")]
    public int MaxPlayers { get; set; }
}
