using BattleTanks.Application.DTOs.Users;
using BattleTanks.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BattleTanks.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserResponseDTO>> GetUser()
    {
        try
        {
            var currentUserFirebaseUid = User.FindFirst("user_id")?.Value;
            var response = await _userService.GetUserAsync(currentUserFirebaseUid);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<UserResponseDTO>> UpdateProfile([FromBody] UpdateUserRequestDTO request)
    {
        try
        {
            var currentUserFirebaseUid = User.FindFirst("user_id")?.Value;

            var response = await _userService.UpdateUserAsync(request, currentUserFirebaseUid);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser()
    {
        try
        {
            var currentUserFirebaseUid = User.FindFirst("user_id")?.Value;

            await _userService.DeleteUserAsync(currentUserFirebaseUid);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
