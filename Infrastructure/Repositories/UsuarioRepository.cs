using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using APIUsuarios.Application.Common;
using APIUsuarios.Application.Exceptions;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;

namespace APIUsuarios.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Usuarios
            .Where(u => u.Ativo)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id && u.Ativo, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var emailNormalizado = EmailNormalizer.Normalize(email);

        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado, ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
    }

    public void Update(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        var emailNormalizado = EmailNormalizer.Normalize(email);

        return await _context.Usuarios
            .AnyAsync(u => u.Email == emailNormalizado, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqliteException { SqliteErrorCode: 19 } sqliteException &&
            sqliteException.Message.Contains("Usuarios.Email", StringComparison.OrdinalIgnoreCase))
        {
            throw new EmailDuplicadoException(ex);
        }
    }
}
