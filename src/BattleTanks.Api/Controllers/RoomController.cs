using BattleTanks.Application.DTOs;
using BattleTanks.Application.DTOs.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Application.Interfaces.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BattleTanks.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IGameSessionService _gameSessionService;

    public RoomController(IRoomService roomService, IGameSessionService gameSessionService)
    {
        _roomService = roomService;
        _gameSessionService = gameSessionService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoomResponseDTO>> CreateRoom([FromBody] CreateRoomRequestDTO CreateRoomRequestDTO)
    {
        var room = await _roomService.CreateRoomAsync(CreateRoomRequestDTO);
        return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResult<RoomResponseDTO>>> GetAllRooms([FromQuery] RoomQueryParams queryParams)
    {
        var result = await _roomService.GetAllRoomsAsync(queryParams);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RoomWithPlayersResponseDTO>> GetRoomById(Guid id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);            
        return Ok(room);
    }

    [HttpPost("{id}/start")]
    [Authorize]
    public async Task<IActionResult> StartGame(Guid id)
    {
        await _gameSessionService.StartGameAsync(id);
        return Ok(new { message = "Game started successfully" });
    }

    [HttpPost("{id}/join")]
    [Authorize]
    public async Task<IActionResult> JoinRoom(Guid id)
    {
        await _roomService.JoinRoomAsync(id);
        return Ok(new { message = "Joined room successfully" });
    }

    [HttpDelete("{id}/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveRoom(Guid id)
    {
        await _roomService.LeaveRoomAsync(id);
        return NoContent();
    }
}
