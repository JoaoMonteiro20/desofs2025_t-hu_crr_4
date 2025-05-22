using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // continua para o próximo middleware
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("A resposta já foi iniciada, não é possível manipular o erro.");
                throw;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json; charset=utf-8";

            var (statusCode, title) = ex switch
            {
                ArgumentException or ValidationException => (HttpStatusCode.BadRequest, "Dados inválidos"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Não autorizado"),
                _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor")
            };

            context.Response.StatusCode = (int)statusCode;

            _logger.Log(
                statusCode == HttpStatusCode.InternalServerError ? LogLevel.Error : LogLevel.Warning,
                ex,
                "Erro tratado pelo middleware: {Message}",
                ex.Message
            );

            var problem = new
            {
                type = $"https://httpstatuses.io/{(int)statusCode}",
                title,
                status = (int)statusCode,
                detail = ex.Message,
                instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}
