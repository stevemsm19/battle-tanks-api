using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BattleTanks.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoomDTO>> CreateRoom([FromBody] CreateRoomDTO createRoomDto)
    {
        if (string.IsNullOrWhiteSpace(createRoomDto.Name))
            return BadRequest(new { message = "Room name is required." });

        var firebaseUid = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        if (string.IsNullOrEmpty(firebaseUid))
            return NotFound(new { message = "User not found." });

        var room = await _roomService.CreateRoomAsync(createRoomDto, firebaseUid);
        return CreatedAtAction(nameof(GetAllRooms), new { id = room.Id }, room);
    }

    [HttpPut("{id}/join")]
    [Authorize]
    public async Task<IActionResult> JoinRoom(Guid id)
    {
        var firebaseUid = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        
        if (string.IsNullOrEmpty(firebaseUid))
            return NotFound(new { message = "User not found." });

        try
        {
            await _roomService.JoinRoomAsync(id, firebaseUid);
            return Ok(new { message = "Joined room successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
