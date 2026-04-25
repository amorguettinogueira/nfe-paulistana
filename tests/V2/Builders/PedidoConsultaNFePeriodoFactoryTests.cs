using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoConsultaNFePeriodoFactory"/>.
/// </summary>
public sealed class PedidoConsultaNFePeriodoFactoryTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    [Fact]
    public void Construtor_CertificadoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new PedidoConsultaNFePeriodoFactory(null!));
    }

    [Fact]
    public void Construtor_CertificadoValido_NaoLancaExcecao()
    {
        // Arrange & Act
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCpf(null!, cpfCnpj, null, dtInicio, dtFim, numeroPagina));
    }

    [Fact]
    public void NewCpf_CpfCnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618);
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCpf(cpf, null!, null, dtInicio, dtFim, numeroPagina));
    }

    [Fact]
    public void NewCpf_DtInicioNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618);
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCpf(cpf, cpfCnpj, null, null!, dtFim, numeroPagina));
    }

    [Fact]
    public void NewCpf_DtFimNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618);
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCpf(cpf, cpfCnpj, null, dtInicio, null!, numeroPagina));
    }

    [Fact]
    public void NewCpf_NumeroPaginaNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618);
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCpf(cpf, cpfCnpj, null, dtInicio, dtFim, null!));
    }

    [Fact]
    public void NewCpf_ComParametrosValidos_RetornaPedidoAssinado()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpf = new Cpf(46381819618);
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act
        var pedido = factory.NewCpf(cpf, cpfCnpj, null, dtInicio, dtFim, numeroPagina);

        // Assert
        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cabecalho);
        Assert.Equal(cpfCnpj, pedido.Cabecalho.CpfCnpj);
        Assert.Equal(dtInicio, pedido.Cabecalho.DtInicio);
        Assert.Equal(dtFim, pedido.Cabecalho.DtFim);
        Assert.Equal(numeroPagina, pedido.Cabecalho.NumeroPagina);
    }

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCnpj(null!, cpfCnpj, null, dtInicio, dtFim, numeroPagina));
    }

    [Fact]
    public void NewCnpj_CpfCnpjNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCnpj(cnpj, null!, null, dtInicio, dtFim, numeroPagina));
    }

    [Fact]
    public void NewCnpj_DtInicioNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCnpj(cnpj, cpfCnpj, null, null!, dtFim, numeroPagina));
    }

    [Fact]
    public void NewCnpj_DtFimNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var numeroPagina = new Numero(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCnpj(cnpj, cpfCnpj, null, dtInicio, null!, numeroPagina));
    }

    [Fact]
    public void NewCnpj_NumeroPaginaNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            factory.NewCnpj(cnpj, cpfCnpj, null, dtInicio, dtFim, null!));
    }

    [Fact]
    public void NewCnpj_ComParametrosValidos_RetornaPedidoAssinado()
    {
        // Arrange
        var factory = new PedidoConsultaNFePeriodoFactory(fixture.Certificado);
        var cnpj = new Cnpj("12345678000195");
        var cpfCnpj = new CpfOrCnpj(new Cpf(46381819618));
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);

        // Act
        var pedido = factory.NewCnpj(cnpj, cpfCnpj, null, dtInicio, dtFim, numeroPagina);

        // Assert
        Assert.NotNull(pedido);
        Assert.NotNull(pedido.Cabecalho);
        Assert.Equal(cpfCnpj, pedido.Cabecalho.CpfCnpj);
        Assert.Equal(dtInicio, pedido.Cabecalho.DtInicio);
        Assert.Equal(dtFim, pedido.Cabecalho.DtFim);
        Assert.Equal(numeroPagina, pedido.Cabecalho.NumeroPagina);
    }
}
