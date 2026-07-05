using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Interfaces;

public interface IPasswordService
{
    string Hash(Usuario usuario, string senha);
}
