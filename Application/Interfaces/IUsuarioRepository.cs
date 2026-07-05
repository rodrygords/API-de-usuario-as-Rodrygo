using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(Usuario usuario, CancellationToken ct);
    void Update(Usuario usuario);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
