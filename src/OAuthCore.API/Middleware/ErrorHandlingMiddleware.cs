using System.Net;
using System.Text.Json;
using OAuthCore.Application.DTOs;
using OAuthCore.Domain.Exceptions;

namespace OAuthCore.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var response = new ErrorResponseDto(
            type: "https://tools.ietf.org/html/rfc6749#section-5.2",
            title: GetTitle(exception),
            status: statusCode,
            detail: exception.Message,
            instance: context.Request.Path
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            OAuthException oAuthException => oAuthException.StatusCode,
            _ => (int)HttpStatusCode.InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            InvalidClientException => "invalid_client",
            InvalidGrantException => "invalid_grant",
            _ => "server_error"
        };
}