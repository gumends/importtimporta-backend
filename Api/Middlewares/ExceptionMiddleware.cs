using Domain.Entities;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
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
            Console.WriteLine(ex);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var isDevelopment = context.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();

        HttpStatusCode status;
        string message = exception.Message;

        switch (exception)
        {
            case BadRequestException:
                status = HttpStatusCode.BadRequest;
                break;

            case UnauthorizedException:
                status = HttpStatusCode.Unauthorized;
                break;

            case NotFoundException:
                status = HttpStatusCode.NotFound;
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                message = isDevelopment
                    ? exception.Message
                    : "Ocorreu um erro interno. Tente novamente mais tarde.";
                break;
        }

        var response = new
        {
            statusCode = (int)status,
            message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response)
        );
    }
}
