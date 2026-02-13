using System.Text.Json;

namespace BattleTanks.Api.Helpers;

public static class ErrorResponseHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task WriteErrorResponseAsync(HttpResponse response, int statusCode, string message, string detail)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";

        var body = new
        {
            code = statusCode,
            message,
            detail
        };

        await response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
