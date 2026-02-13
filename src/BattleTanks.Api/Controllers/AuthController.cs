using BattleTanks.Application.DTOs.Auth;
using BattleTanks.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BattleTanks.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDTO>> Register([FromBody] RegisterRequestDTO request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }
}

