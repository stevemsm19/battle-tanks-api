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

    [HttpGet("{firebaseUid}")]
    [Authorize]
    public async Task<ActionResult<UserResponseDTO>> GetUser(string firebaseUid)
    {
        try
        {
            var response = await _userService.GetUserAsync(firebaseUid);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{firebaseUid}")]
    [Authorize]
    public async Task<ActionResult<UserResponseDTO>> UpdateProfile(string firebaseUid, [FromBody] UpdateUserRequestDTO request)
    {
        try
        {
            var currentUserFirebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _userService.UpdateUserAsync(firebaseUid, request, currentUserFirebaseUid);
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

    [HttpDelete("{firebaseUid}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(string firebaseUid)
    {
        try
        {
            var currentUserFirebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _userService.DeleteUserAsync(firebaseUid, currentUserFirebaseUid);
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
