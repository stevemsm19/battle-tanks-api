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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RoomDetailDTO>> GetRoomById(Guid id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);            
        return Ok(room);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoomDTO>> CreateRoom([FromBody] CreateRoomDTO createRoomDto)
    {
        var room = await _roomService.CreateRoomAsync(createRoomDto);
        return CreatedAtAction(nameof(GetAllRooms), new { id = room.Id }, room);
    }

    [HttpPost("join")]
    [Authorize]
    public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequestDTO request)
    {
        await _roomService.JoinRoomAsync(request.RoomId);
        return Ok(new { message = "Joined room successfully" });
    }
}
