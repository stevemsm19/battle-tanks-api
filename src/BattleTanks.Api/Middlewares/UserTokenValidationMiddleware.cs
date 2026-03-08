using BattleTanks.Api.Helpers;
using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Exceptions;
using BattleTanks.Domain.Interfaces.Users;

namespace BattleTanks.Api.Middlewares;

public class UserTokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public UserTokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository, IUserContext userContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var firebaseUid = context.User.FindFirst("user_id")?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                throw new BadRequestException("Invalid token: missing user identifier.");
            }

            var user = await userRepository.GetByFirebaseUidAsync(firebaseUid);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            userContext.CurrentUser = user;
        }

        await _next(context);
    }
}
