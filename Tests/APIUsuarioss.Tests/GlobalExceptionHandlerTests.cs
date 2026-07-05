using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using APIUsuarios.Application.Exceptions;
using APIUsuarios.Infrastructure.Errors;

namespace APIUsuarioss.Tests;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_NaoExpoeDetalhesDeErroInesperado()
    {
        var problemDetailsService = new CapturingProblemDetailsService();
        var handler = new GlobalExceptionHandler(
            problemDetailsService,
            NullLogger<GlobalExceptionHandler>.Instance);
        var httpContext = new DefaultHttpContext();

        var tratado = await handler.TryHandleAsync(
            httpContext,
            new InvalidOperationException("detalhe interno sensível"),
            CancellationToken.None);

        Assert.True(tratado);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.NotNull(problemDetailsService.Captured);
        Assert.Equal("Erro interno do servidor", problemDetailsService.Captured.Title);
        Assert.DoesNotContain(
            "detalhe interno sensível",
            problemDetailsService.Captured.Detail,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task TryHandleAsync_ConverteEmailDuplicadoEmConflict()
    {
        var problemDetailsService = new CapturingProblemDetailsService();
        var handler = new GlobalExceptionHandler(
            problemDetailsService,
            NullLogger<GlobalExceptionHandler>.Instance);
        var httpContext = new DefaultHttpContext();

        await handler.TryHandleAsync(
            httpContext,
            new EmailDuplicadoException(),
            CancellationToken.None);

        Assert.Equal(StatusCodes.Status409Conflict, httpContext.Response.StatusCode);
        Assert.Equal("Email já cadastrado", problemDetailsService.Captured?.Detail);
    }

    private sealed class CapturingProblemDetailsService : IProblemDetailsService
    {
        public ProblemDetails? Captured { get; private set; }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            Captured = context.ProblemDetails;
            return ValueTask.FromResult(true);
        }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            Captured = context.ProblemDetails;
            return ValueTask.CompletedTask;
        }
    }
}
