using BattleTanks.Api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BattleTanks.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"] ?? throw new InvalidOperationException("Firebase ProjectId is not configured");

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
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/gameHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        var detail = context.Error switch
                        {
                            "invalid_token" when context.ErrorDescription?.Contains("expired") == true
                                => "The token has expired.",
                            "invalid_token"
                                => "The token is invalid.",
                            _
                                => "Authentication is required to access this resource."
                        };

                        await ErrorResponseHelper.WriteErrorResponseAsync(context.Response, 401, "Authentication error", detail);
                    },
                    OnForbidden = async context =>
                    {
                        await ErrorResponseHelper.WriteErrorResponseAsync(context.Response, 403, "Access denied", "You do not have permission to access this resource.");
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}


