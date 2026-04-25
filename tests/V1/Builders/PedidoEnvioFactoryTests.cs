using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;

namespace Nfe.Paulistana.Tests.V1.Builders;

/// <summary>
/// Testes unitários para <see cref="PedidoEnvioFactory"/>:
/// guard clauses do construtor e dos métodos de fábrica, e verificação
/// de que o pedido retornado está corretamente assinado.
/// </summary>
public class PedidoEnvioFactoryTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_ConfiguracaoNula_ThrowsArgumentNullException()
    {
        Certificado? config = null;

        _ = Assert.Throws<ArgumentNullException>(() => new PedidoEnvioFactory(config!));
    }

    // ============================================
    // NewCpf — guard clauses
    // ============================================

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf(cpf!, RpsTestFactory.Padrao()));
    }

    [Fact]
    public void NewCpf_RpsNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);
        Rps? rps = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, rps!));
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao());
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_RpsContidoTemAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao());
        Assert.NotNull(resultado.Rps?.Assinatura);
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_CabecalhoContemCpfCorreto()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCpf(cpf, RpsTestFactory.Padrao());

        Assert.Equal(cpf.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cpf?.ToString());
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_CabecalhoNaoContemCnpj()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao());
        Assert.Null(resultado.Cabecalho?.CpfOrCnpj?.Cnpj);
    }

    // ============================================
    // NewCnpj
    // ============================================

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);
        Cnpj? cnpj = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCnpj(cnpj!, RpsTestFactory.Padrao()));
    }

    [Fact]
    public void NewCnpj_RpsNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);
        Rps? rps = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCnpj((Cnpj)Helpers.TestConstants.ValidCnpj, rps!));
    }

    [Fact]
    public void NewCnpj_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCnpj((Cnpj)Helpers.TestConstants.ValidCnpj, RpsTestFactory.Padrao());
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Fact]
    public void NewCnpj_ArgumentosValidos_RpsContidoTemAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCnpj((Cnpj)Helpers.TestConstants.ValidCnpj, RpsTestFactory.Padrao());
        Assert.NotNull(resultado.Rps?.Assinatura);
    }

    [Fact]
    public void NewCnpj_ArgumentosValidos_CabecalhoContemCnpjCorreto()
    {
        var cnpj = (Cnpj)Helpers.TestConstants.ValidCnpj;
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        PedidoEnvio resultado = factory.NewCnpj(cnpj, RpsTestFactory.Padrao());

        Assert.Equal(cnpj.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cnpj?.ToString());
    }
}