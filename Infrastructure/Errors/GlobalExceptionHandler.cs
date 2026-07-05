using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using APIUsuarios.Application.Exceptions;

namespace APIUsuarios.Infrastructure.Errors;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            EmailDuplicadoException => (
                StatusCodes.Status409Conflict,
                "Conflito",
                exception.Message),
            UsuarioNaoEncontradoException => (
                StatusCodes.Status404NotFound,
                "Recurso não encontrado",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Erro não tratado na requisição {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogInformation(
                "Requisição rejeitada com status {StatusCode} por {ExceptionType}",
                status,
                exception.GetType().Name);
        }

        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        var written = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        if (!written)
        {
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }
}
