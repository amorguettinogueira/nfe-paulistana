using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
namespace Nfe.Paulistana.Tests.V2.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoEnvioLoteFactory"/>.
/// </summary>
public sealed class PedidoEnvioLoteFactoryTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{

    [Fact]
    public void Construtor_CertificadoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new PedidoEnvioLoteFactory(null!));
    }

    [Fact]
    public void Construtor_CertificadoValido_NaoLancaExcecao()
    {
        // Arrange & Act
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var rps = new Rps();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(null!, false, [rps]));
    }

    [Fact]
    public void NewCpf_RpsListNula_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L); // CPF válido para teste

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(cpf, false, null!));
    }

    [Fact]
    public void NewCpf_RpsListVazia_ThrowsArgumentException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L); // CPF válido para teste

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.NewCpf(cpf, false, []));
        Assert.Contains("pelo menos uma RPS", exception.Message);
    }

    [Fact]
    public void NewCpf_RpsListComApenasNulos_ThrowsArgumentException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L); // CPF válido para teste

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.NewCpf(cpf, false, [null!, null!]));
        Assert.Contains("pelo menos uma RPS", exception.Message);
    }

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var rps = new Rps();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(null!, false, [rps]));
    }

    [Fact]
    public void NewCnpj_RpsListNula_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(cnpj, false, null!));
    }

    [Fact]
    public void NewCnpj_RpsListVazia_ThrowsArgumentException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.NewCnpj(cnpj, false, []));
        Assert.Contains("pelo menos uma RPS", exception.Message);
    }
}
