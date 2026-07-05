using Microsoft.AspNetCore.Identity;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Infrastructure.Security;

public sealed class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public PasswordService(IPasswordHasher<Usuario> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Hash(Usuario usuario, string senha) =>
        _passwordHasher.HashPassword(usuario, senha);
}
