using BattleTanks.Api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BattleTanks.Api.Extensions;

public static class AuthenticationExtensions
{
    private const string SignalrHubPath = "/gameHub";
    private const string AccessTokenQueryParam = "access_token";

    public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"] 
            ?? throw new InvalidOperationException("Firebase ProjectId is not configured");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{projectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = HandleSignalrToken,
                    OnChallenge = context => HandleAuthChallenge(context),
                    OnForbidden = context => 
                        ErrorResponseHelper.WriteErrorResponseAsync(
                            context.Response, 403, "Access denied", "No permission.")
                };
            });

        services.AddAuthorization();
        return services;
    }

    private static Task HandleSignalrToken(MessageReceivedContext context)
    {
        var accessToken = context.Request.Query[AccessTokenQueryParam];
        var path = context.HttpContext.Request.Path;

        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(SignalrHubPath))
        {
            context.Token = accessToken;
        }

        return Task.CompletedTask;
    }

    private static async Task HandleAuthChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();

        var (title, detail) = context.Error switch
        {
            "invalid_token" when context.ErrorDescription?.Contains("expired") == true 
                => ("Expired Token", "The session has expired."),
            "invalid_token" 
                => ("Invalid Token", "The security token is invalid."),
            _ 
                => ("Unauthorized", "Authentication is required.")
        };

        await ErrorResponseHelper.WriteErrorResponseAsync(context.Response, 401, title, detail);
    }
}