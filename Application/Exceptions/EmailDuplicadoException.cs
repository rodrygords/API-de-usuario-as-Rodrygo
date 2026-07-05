namespace APIUsuarios.Application.Exceptions;

public sealed class EmailDuplicadoException : Exception
{
    public EmailDuplicadoException()
        : base("Email já cadastrado")
    {
    }

    public EmailDuplicadoException(Exception innerException)
        : base("Email já cadastrado", innerException)
    {
    }
}
