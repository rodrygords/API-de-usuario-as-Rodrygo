using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Infrastructure.Repositories;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Converters;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Errors;
using APIUsuarios.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

    options.SerializerOptions.Converters.Add(new DateTimeConverter());
    options.SerializerOptions.Converters.Add(new NullableDateTimeConverter());
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    var usuarios = await service.ListarAsync(ct);
    return Results.Ok(usuarios);
})
.WithName("ListarUsuarios");

app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.ObterAsync(id, ct);
    return usuario != null
        ? Results.Ok(usuario)
        : UsuarioNotFoundProblem();
})
.WithName("ObterUsuario");

app.MapPost("/usuarios", async (
    UsuarioCreateDto dto,
    IUsuarioService service,
    IValidator<UsuarioCreateDto> validator,
    CancellationToken ct) =>
{
    var validationResult = await validator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(ToValidationErrors(validationResult));
    }

    var usuario = await service.CriarAsync(dto, ct);
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
})
.WithName("CriarUsuario");

app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IUsuarioService service,
    IValidator<UsuarioUpdateDto> validator,
    CancellationToken ct) =>
{
    var validationResult = await validator.ValidateAsync(dto, ct);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(ToValidationErrors(validationResult));
    }

    var usuario = await service.AtualizarAsync(id, dto, ct);
    return Results.Ok(usuario);
})
.WithName("AtualizarUsuario");

app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var removido = await service.RemoverAsync(id, ct);
    return removido
        ? Results.NoContent()
        : UsuarioNotFoundProblem();
})
.WithName("RemoverUsuario");

app.Run();

static Dictionary<string, string[]> ToValidationErrors(FluentValidation.Results.ValidationResult result) =>
    result.Errors
        .GroupBy(error => error.PropertyName)
        .ToDictionary(
            group => group.Key,
            group => group.Select(error => error.ErrorMessage).ToArray());

static IResult UsuarioNotFoundProblem() =>
    Results.Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Recurso não encontrado",
        detail: "Usuário não encontrado");
