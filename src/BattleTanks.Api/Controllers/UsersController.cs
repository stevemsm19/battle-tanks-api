using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BattleTanks.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDTO>> GetUser()
    {
        var response = await _userService.GetUserAsync();
        return Ok(response);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<UserResponseDTO>> UpdateProfile([FromBody] UpdateUserRequestDTO request)
    {
        var response = await _userService.UpdateUserAsync(request);
        return Ok(response);
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser()
    {
        await _userService.DeleteUserAsync();
        return Ok(new { message = "User deleted successfully" });
    }
}
