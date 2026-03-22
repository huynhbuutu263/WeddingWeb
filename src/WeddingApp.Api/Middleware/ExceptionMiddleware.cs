using System.Net;
using System.Text.Json;
using WeddingApp.Application.Common.Exceptions;

namespace WeddingApp.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation error occurred.");
            await WriteProblemAsync(context, (int)HttpStatusCode.BadRequest, new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "One or more validation errors occurred.",
                status = (int)HttpStatusCode.BadRequest,
                errors = ex.Errors
            });
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found.");
            await WriteProblemAsync(context, (int)HttpStatusCode.NotFound, new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = "The specified resource was not found.",
                status = (int)HttpStatusCode.NotFound,
                detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");
            await WriteProblemAsync(context, (int)HttpStatusCode.InternalServerError, new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "An error occurred while processing your request.",
                status = (int)HttpStatusCode.InternalServerError
            });
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, object problem)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}

