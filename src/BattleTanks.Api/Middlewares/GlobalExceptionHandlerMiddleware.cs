using BattleTanks.Api.Helpers;
using BattleTanks.Application.Exceptions;
using BattleTanks.Domain.Exceptions;
using System.Net;

namespace BattleTanks.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            ExternalServiceException => (int)HttpStatusCode.BadGateway,
            AppException => (int)HttpStatusCode.BadRequest,
            DomainException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var detail = exception.Message;
        await ErrorResponseHelper.WriteErrorResponseAsync(context.Response, statusCode, "An error occurred", detail);
    }
}

