namespace APIUsuarios.Application.Exceptions;

public sealed class UsuarioNaoEncontradoException : Exception
{
    public UsuarioNaoEncontradoException()
        : base("Usuário não encontrado")
    {
    }
}
