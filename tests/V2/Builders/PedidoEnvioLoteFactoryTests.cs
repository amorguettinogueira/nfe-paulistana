using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V2.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoEnvioLoteFactory"/>.
/// </summary>
public sealed class PedidoEnvioLoteFactoryTests
{
    private static Certificado CriarConfiguracao()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Teste", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return new Certificado
        {
            Certificate = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1))
        };
    }

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
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var rps = new Rps();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(null!, false, [rps]));
    }

    [Fact]
    public void NewCpf_RpsListNula_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var cpf = new Cpf(46381819618L); // CPF válido para teste

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(cpf, false, null!));
    }

    [Fact]
    public void NewCpf_RpsListVazia_ThrowsArgumentException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var cpf = new Cpf(46381819618L); // CPF válido para teste

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.NewCpf(cpf, false, []));
        Assert.Contains("pelo menos uma RPS", exception.Message);
    }

    [Fact]
    public void NewCpf_RpsListComApenasNulos_ThrowsArgumentException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var cpf = new Cpf(46381819618L); // CPF válido para teste

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.NewCpf(cpf, false, [null!, null!]));
        Assert.Contains("pelo menos uma RPS", exception.Message);
    }

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var rps = new Rps();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(null!, false, [rps]));
    }

    [Fact]
    public void NewCnpj_RpsListNula_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var cnpj = new Cnpj("12345678000195");

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(cnpj, false, null!));
    }

    [Fact]
    public void NewCnpj_RpsListVazia_ThrowsArgumentException()
    {
        // Arrange
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        var cnpj = new Cnpj("12345678000195");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factory.NewCnpj(cnpj, false, []));
        Assert.Contains("pelo menos uma RPS", exception.Message);
    }
}
