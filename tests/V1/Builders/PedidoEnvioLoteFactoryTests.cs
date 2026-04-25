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
/// Testes unitários para <see cref="PedidoEnvioLoteFactory"/>:
/// guard clauses, assinatura de todos os RPS, e verificação das totalizações
/// automáticas de <see cref="CabecalhoLote"/>.
/// </summary>
public class PedidoEnvioLoteFactoryTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_ConfiguracaoNula_ThrowsArgumentNullException()
    {
        Certificado? config = null;

        _ = Assert.Throws<ArgumentNullException>(() => new PedidoEnvioLoteFactory(config!));
    }

    // ============================================
    // NewCpf — guard clauses
    // ============================================

    [Fact]
    public void NewCpf_CpfNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf(cpf!, false, [RpsTestFactory.Padrao()]));
    }

    [Fact]
    public void NewCpf_RpsListaNula_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, null!));
    }

    [Fact]
    public void NewCpf_RpsListaVazia_ThrowsArgumentException()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        _ = Assert.Throws<ArgumentException>(() => factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, []));
    }

    [Fact]
    public void NewCpf_RpsListaComElementoNulo_ThrowsArgumentException()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        _ = Assert.Throws<ArgumentException>(() => factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [null!]));
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao()]);
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_CabecalhoContemCpfCorreto()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        PedidoEnvioLote resultado = factory.NewCpf(cpf, false, [RpsTestFactory.Padrao()]);

        Assert.Equal(cpf.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cpf?.ToString());
    }

    [Fact]
    public void NewCpf_ArgumentosValidos_CabecalhoNaoContemCnpj()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao()]);
        Assert.Null(resultado.Cabecalho?.CpfOrCnpj?.Cnpj);
    }

    // ============================================
    // NewCnpj — guard clauses
    // ============================================

    [Fact]
    public void NewCnpj_CnpjNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Cnpj? cnpj = null;
        Rps rps = RpsTestFactory.Padrao();

        _ = Assert.Throws<ArgumentNullException>(() => factory.NewCnpj(cnpj!, false, [rps]));
    }

    [Fact]
    public void NewCnpj_RpsNulo_ThrowsArgumentNullException()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps? rps = null;

        _ = Assert.Throws<ArgumentException>(() => factory.NewCnpj((Cnpj)Helpers.TestConstants.ValidCnpj, false, [rps!]));
    }

    [Fact]
    public void NewCnpj_ArgumentosValidos_RetornaPedidoEnvioComAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        PedidoEnvioLote resultado = factory.NewCnpj((Cnpj)Helpers.TestConstants.ValidCnpj, false, [RpsTestFactory.Padrao()]);
        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Fact]
    public void NewCnpj_ArgumentosValidos_CabecalhoContemCnpjCorreto()
    {
        var cnpj = (Cnpj)Helpers.TestConstants.ValidCnpj;
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        PedidoEnvioLote resultado = factory.NewCnpj(cnpj, false, [RpsTestFactory.Padrao()]);

        Assert.Equal(cnpj.ToString(), resultado.Cabecalho?.CpfOrCnpj?.Cnpj?.ToString());
    }

    [Fact]
    public void NewCnpj_ArgumentosValidos_CabecalhoNaoContemCpf()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);

        PedidoEnvioLote resultado = factory.NewCnpj((Cnpj)Helpers.TestConstants.ValidCnpj, false, [RpsTestFactory.Padrao()]);
        Assert.Null(resultado.Cabecalho?.CpfOrCnpj?.Cpf);
    }

    // ============================================
    // NewCpf — assinatura
    // ============================================

    [Fact]
    public void NewCpf_RpsValido_LoteTemAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps = RpsTestFactory.Padrao();

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps]);

        Assert.NotNull(resultado.SignedXmlContent);
    }

    [Fact]
    public void NewCpf_RpsValido_RpsContidoTemAssinaturaPreenchida()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps = RpsTestFactory.Padrao();

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps]);

        Assert.All(resultado.Rps!, r => Assert.NotNull(r.Assinatura));
    }

    // ============================================
    // NewCpf — totalizações automáticas
    // ============================================

    [Fact]
    public void NewCpf_DoisRps_QtdRpsIgualADois()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps1 = RpsTestFactory.Padrao(valorServicos: 1000m, dataEmissao: new DateTime(2024, 1, 1));
        Rps rps2 = RpsTestFactory.Padrao(valorServicos: 2000m, dataEmissao: new DateTime(2024, 1, 31));

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps1, rps2]);

        Assert.Equal("2", resultado.Cabecalho?.QtdRps?.ToString());
    }

    [Fact]
    public void NewCpf_DoisRps_ValorTotalServicosEhSomaDosValores()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps1 = RpsTestFactory.Padrao(valorServicos: 1000m, dataEmissao: new DateTime(2024, 1, 1));
        Rps rps2 = RpsTestFactory.Padrao(valorServicos: 2000m, dataEmissao: new DateTime(2024, 1, 31));

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps1, rps2]);

        Assert.Equal(new Valor(3000m).ToString(), resultado.Cabecalho?.ValorTotalServicos?.ToString());
    }

    [Fact]
    public void NewCpf_SemDeducoes_ValorTotalDeducoesEhNulo()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps = RpsTestFactory.Padrao();

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps]);

        Assert.Null(resultado.Cabecalho?.ValorTotalDeducoes);
    }

    [Fact]
    public void NewCpf_DoisRps_DtInicioEhDataMaisAntiga()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var dataInicio = new DateTime(2024, 1, 1);
        Rps rps1 = RpsTestFactory.Padrao(valorServicos: 1000m, dataEmissao: dataInicio);
        Rps rps2 = RpsTestFactory.Padrao(valorServicos: 2000m, dataEmissao: new DateTime(2024, 1, 31));

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps1, rps2]);

        Assert.Equal(new DataXsd(dataInicio).ToString(), resultado.Cabecalho?.DtInicio?.ToString());
    }

    [Fact]
    public void NewCpf_DoisRps_DtFimEhDataMaisRecente()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        var dataFim = new DateTime(2024, 1, 31);
        Rps rps1 = RpsTestFactory.Padrao(valorServicos: 1000m, dataEmissao: new DateTime(2024, 1, 1));
        Rps rps2 = RpsTestFactory.Padrao(valorServicos: 2000m, dataEmissao: dataFim);

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps1, rps2]);

        Assert.Equal(new DataXsd(dataFim).ToString(), resultado.Cabecalho?.DtFim?.ToString());
    }

    [Fact]
    public void NewCpf_Transacao_PropagadaParaCabecalho()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps = RpsTestFactory.Padrao();

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, true, [rps]);

        Assert.True(resultado.Cabecalho?.Transacao);
    }

    [Fact]
    public void NewCpf_DoisRps_ArrayRpsContemAmbos()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        Rps rps1 = RpsTestFactory.Padrao(valorServicos: 1000m, dataEmissao: new DateTime(2024, 1, 1));
        Rps rps2 = RpsTestFactory.Padrao(valorServicos: 2000m, dataEmissao: new DateTime(2024, 1, 31));

        PedidoEnvioLote resultado = factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps1, rps2]);

        Assert.Equal(2, resultado.Rps?.Length);
    }
}