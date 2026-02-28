using BattleTanks.Api.Helpers;
using BattleTanks.Application.Interfaces;
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
                await ErrorResponseHelper.WriteErrorResponseAsync(
                    context.Response, 401, "Authentication error", "Invalid token: missing user identifier.");
                return;
            }

            var user = await userRepository.GetByFirebaseUidAsync(firebaseUid);

            if (user == null)
            {
                await ErrorResponseHelper.WriteErrorResponseAsync(
                    context.Response, 401, "Authentication error", "User without permissions.");
                return;
            }

            userContext.CurrentUser = user;
        }

        await _next(context);
    }
}
