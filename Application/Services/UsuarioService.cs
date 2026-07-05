using APIUsuarios.Application.Common;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Exceptions;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordService _passwordService;

    public UsuarioService(
        IUsuarioRepository repository,
        IPasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repository.GetAllAsync(ct);
        return usuarios.Select(MapToReadDto);
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        return usuario != null ? MapToReadDto(usuario) : null;
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        var emailNormalizado = EmailNormalizer.Normalize(dto.Email);

        if (await _repository.EmailExistsAsync(emailNormalizado, ct))
        {
            throw new EmailDuplicadoException();
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = emailNormalizado,
            SenhaHash = string.Empty,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        usuario.SenhaHash = _passwordService.Hash(usuario, dto.Senha);

        await _repository.AddAsync(usuario, ct);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null)
        {
            throw new UsuarioNaoEncontradoException();
        }

        var emailNormalizado = EmailNormalizer.Normalize(dto.Email);
        var usuarioComEmail = await _repository.GetByEmailAsync(emailNormalizado, ct);
        if (usuarioComEmail != null && usuarioComEmail.Id != id)
        {
            throw new EmailDuplicadoException();
        }

        usuario.Nome = dto.Nome;
        usuario.Email = emailNormalizado;
        usuario.DataNascimento = dto.DataNascimento;
        usuario.Telefone = dto.Telefone;
        usuario.Ativo = dto.Ativo;
        usuario.DataAtualizacao = DateTime.UtcNow;

        _repository.Update(usuario);
        await _repository.SaveChangesAsync(ct);

        return MapToReadDto(usuario);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var usuario = await _repository.GetByIdAsync(id, ct);
        if (usuario == null)
        {
            return false;
        }

        usuario.Ativo = false;
        usuario.DataAtualizacao = DateTime.UtcNow;

        _repository.Update(usuario);
        await _repository.SaveChangesAsync(ct);

        return true;
    }

    private static UsuarioReadDto MapToReadDto(Usuario usuario)
    {
        return new UsuarioReadDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataNascimento,
            usuario.Telefone,
            usuario.Ativo,
            usuario.DataCriacao
        );
    }
}
