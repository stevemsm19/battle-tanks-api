using BattleTanks.Application.DTOs.Maps;
using BattleTanks.Application.Interfaces.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BattleTanks.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameMapController : ControllerBase
{
    private readonly IGameMapService _gameMapService;

    public GameMapController(IGameMapService gameMapService)
    {
        _gameMapService = gameMapService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<GameMapDTO>>> GetAllMaps()
    {
        var maps = await _gameMapService.GetAllMapsAsync();
        return Ok(maps);
    }
}
