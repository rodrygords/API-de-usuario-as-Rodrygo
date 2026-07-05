using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Validators;
using Xunit;

namespace APIUsuarioss.Tests;

public sealed class UsuarioCreateDtoValidatorTests
{
    [Fact]
    public async Task ValidateAsync_RejeitaDadosInvalidos()
    {
        var validator = new UsuarioCreateDtoValidator();
        var dto = new UsuarioCreateDto(
            "A",
            "email-invalido",
            "123",
            DateTime.Today.AddYears(-15),
            "telefone-invalido");

        var resultado = await validator.ValidateAsync(dto);

        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.PropertyName == nameof(dto.Nome));
        Assert.Contains(resultado.Errors, error => error.PropertyName == nameof(dto.Email));
        Assert.Contains(resultado.Errors, error => error.PropertyName == nameof(dto.Senha));
        Assert.Contains(resultado.Errors, error => error.PropertyName == nameof(dto.DataNascimento));
        Assert.Contains(resultado.Errors, error => error.PropertyName == nameof(dto.Telefone));
    }
}
