using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoConsultaCNPJFactory"/> (V1).
/// </summary>
public sealed class PedidoConsultaCNPJFactoryTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    [Fact]
    public void Construtor_CertificadoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new PedidoConsultaCNPJFactory(null!));
    }

    [Fact]
    public void Construtor_CertificadoValido_NaoLancaExcecao()
    {
        // Arrange & Act
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var contribuinte = new CpfOrCnpj(new Cpf(46381819618L));

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(null!, contribuinte));
    }

    [Fact]
    public void NewCpf_ContribuinteNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(cpf, null!));
    }

    [Fact]
    public void NewCpf_ComParametrosValidos_RetornaPedidoAssinado()
    {
        // Arrange
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L);
        var contribuinte = new CpfOrCnpj(new Cpf(46381819618L));

        // Act
        var pedido = factory.NewCpf(cpf, contribuinte);

        // Assert
        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cabecalho);
        Assert.NotNull(pedido.CnpjContribuinte);
    }

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var contribuinte = new CpfOrCnpj(new Cpf(46381819618L));

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(null!, contribuinte));
    }

    [Fact]
    public void NewCnpj_ContribuinteNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var cnpj = new Cnpj(84067820000190L);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(cnpj, null!));
    }

    [Fact]
    public void NewCnpj_ComParametrosValidos_RetornaPedidoAssinado()
    {
        // Arrange
        var factory = new PedidoConsultaCNPJFactory(fixture.Certificado);
        var cnpj = new Cnpj(84067820000190L);
        var contribuinte = new CpfOrCnpj(new Cnpj(84067820000190L));

        // Act
        var pedido = factory.NewCnpj(cnpj, contribuinte);

        // Assert
        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cabecalho);
        Assert.NotNull(pedido.CnpjContribuinte);
    }
}
