using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Exceptions;
using APIUsuarios.Application.Services;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using APIUsuarios.Infrastructure.Security;

namespace APIUsuarioss.Tests;

public sealed class UsuarioServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var repository = new UsuarioRepository(_context);
        _service = new UsuarioService(
            repository,
            new PasswordService(new PasswordHasher<Usuario>()));
    }

    [Fact]
    public async Task CriarAsync_NormalizaEmailEGeraHashSemExporSenha()
    {
        var dto = CriarDto("  Maria.Silva@Example.COM  ", "senha-segura-123");

        var resultado = await _service.CriarAsync(dto, CancellationToken.None);
        var salvo = await _context.Usuarios.SingleAsync();

        Assert.Equal("maria.silva@example.com", resultado.Email);
        Assert.Equal("maria.silva@example.com", salvo.Email);
        Assert.NotEqual(dto.Senha, salvo.SenhaHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            _passwordHasher.VerifyHashedPassword(salvo, salvo.SenhaHash, dto.Senha));
        Assert.DoesNotContain(
            typeof(UsuarioReadDto).GetProperties(),
            property => property.Name.Contains("Senha", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CriarAsync_RejeitaEmailDuplicadoAposNormalizacao()
    {
        await _service.CriarAsync(
            CriarDto("usuario@example.com", "primeira-senha"),
            CancellationToken.None);

        await Assert.ThrowsAsync<EmailDuplicadoException>(() =>
            _service.CriarAsync(
                CriarDto("  USUARIO@EXAMPLE.COM ", "segunda-senha"),
                CancellationToken.None));
    }

    [Fact]
    public async Task RemoverAsync_RealizaSoftDeleteEOmiteUsuarioDaListagem()
    {
        var criado = await _service.CriarAsync(
            CriarDto("remover@example.com", "senha-segura"),
            CancellationToken.None);

        var removido = await _service.RemoverAsync(criado.Id, CancellationToken.None);
        _context.ChangeTracker.Clear();

        var salvo = await _context.Usuarios.SingleAsync();
        var usuariosAtivos = await _service.ListarAsync(CancellationToken.None);
        var usuarioRemovido = await _service.ObterAsync(criado.Id, CancellationToken.None);

        Assert.True(removido);
        Assert.False(salvo.Ativo);
        Assert.Empty(usuariosAtivos);
        Assert.Null(usuarioRemovido);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private static UsuarioCreateDto CriarDto(string email, string senha) =>
        new(
            "Maria Silva",
            email,
            senha,
            DateTime.Today.AddYears(-25),
            "(51) 99999-9999");
}
