using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoConsultaNFeFactory"/> (V1).
/// </summary>
public sealed class PedidoConsultaNFeFactoryTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    [Fact]
    public void Construtor_CertificadoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new PedidoConsultaNFeFactory(null!));
    }

    [Fact]
    public void Construtor_CertificadoValido_NaoLancaExcecao()
    {
        // Arrange & Act
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var detalhes = new[] { new DetalheConsultaNFe() };

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(null!, detalhes));
    }

    [Fact]
    public void NewCpf_DetalhesNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCpf(cpf, null!));
    }

    [Fact]
    public void NewCpf_ComParametrosValidos_RetornaPedidoAssinado()
    {
        // Arrange
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618L);
        var detalhes = new[] { new DetalheConsultaNFe() };

        // Act
        var pedido = factory.NewCpf(cpf, detalhes);

        // Assert
        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cabecalho);
        Assert.NotNull(pedido.Detalhe);
        Assert.Single(pedido.Detalhe);
    }

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var detalhes = new[] { new DetalheConsultaNFe() };

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(null!, detalhes));
    }

    [Fact]
    public void NewCnpj_DetalhesNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var cnpj = new Cnpj(84067820000190L);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => factory.NewCnpj(cnpj, null!));
    }

    [Fact]
    public void NewCnpj_ComParametrosValidos_RetornaPedidoAssinado()
    {
        // Arrange
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var cnpj = new Cnpj(84067820000190L);
        var detalhes = new[] { new DetalheConsultaNFe() };

        // Act
        var pedido = factory.NewCnpj(cnpj, detalhes);

        // Assert
        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cabecalho);
        Assert.NotNull(pedido.Detalhe);
        Assert.Single(pedido.Detalhe);
    }
}
