using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Builders;

public class RpsBuilderTests
{
    // ============================================
    // Helpers
    // ============================================

    private static readonly InscricaoMunicipal InscricaoPrestador = new(39616924);
    private static readonly Numero NumeroRps = new(4105);
    private static readonly Discriminacao Discriminacao = new("Desenvolvimento de software.");
    private static readonly SerieRps Serie = new("BB");
    private static readonly DataXsd DataEmissao = new(new DateTime(2024, 1, 20));
    private static readonly TributacaoNfe Tributacao = (TributacaoNfe)'T';
    private static readonly CodigoServico CodigoServico = new(7617);
    private static readonly Valor ValorServicos = (Valor)20500m;
    private static readonly Aliquota Aliquota = (Aliquota)0.05m;

    private static readonly Tomador TomadorPadrao =
        TomadorBuilder.NewCpf(new Cpf(new ValidCpfNumber().Min())).Build();

    /// <summary>Retorna a cadeia obrigatória completa até IRpsSetOptionals, com serieRps parametrizável.</summary>
    private static IRpsSetOptionals CadeiaObrigatoria(SerieRps? serie = null) =>
        RpsBuilder.New(InscricaoPrestador, TipoRps.NotaFiscalConjugada, NumeroRps, Discriminacao, serie)
            .SetNFe(DataEmissao, Tributacao, StatusNfe.Normal)
            .SetServico(CodigoServico, ValorServicos)
            .SetIss(Aliquota, false);

    // ============================================
    // New() — Factory Method Tests
    // ============================================

    [Fact]
    public void New_WithAllRequiredParams_ReturnsIRpsSetNfe()
    {
        IRpsSetNfe step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie);

        _ = Assert.IsAssignableFrom<IRpsSetNfe>(step);
    }

    [Fact]
    public void New_WithNullInscricaoPrestador_ThrowsArgumentNullException()
    {
        InscricaoMunicipal? inscricao = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(inscricao!, TipoRps.Rps, NumeroRps, Discriminacao, Serie));
    }

    [Fact]
    public void New_WithNullNumeroRps_ThrowsArgumentNullException()
    {
        Numero? numero = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, numero!, Discriminacao, Serie));
    }

    [Fact]
    public void New_WithNullDiscriminacao_ThrowsArgumentNullException()
    {
        Discriminacao? discriminacao = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, discriminacao!, Serie));
    }

    [Fact]
    public void New_WithNullSerieRps_ReturnsIRpsSetNfe()
    {
        // SerieRps é opcional em New() — apenas Build() exige que seja não-nulo
        IRpsSetNfe step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, null);

        _ = Assert.IsAssignableFrom<IRpsSetNfe>(step);
    }

    // ============================================
    // SetNFe() Tests
    // ============================================

    [Fact]
    public void SetNFe_WithValidParams_ReturnsIRpsSetServico()
    {
        IRpsSetServico step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, StatusNfe.Normal);

        _ = Assert.IsAssignableFrom<IRpsSetServico>(step);
    }

    [Fact]
    public void SetNFe_WithNullDataEmissao_ThrowsArgumentNullException()
    {
        DataXsd? dataEmissao = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(dataEmissao!, Tributacao, StatusNfe.Normal));
    }

    [Fact]
    public void SetNFe_WithNullTributacao_ThrowsArgumentNullException()
    {
        TributacaoNfe? tributacao = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, tributacao!, StatusNfe.Normal));
    }

    // ============================================
    // SetServico() Tests
    // ============================================

    [Fact]
    public void SetServico_WithValidParams_ReturnsIRpsSetIss()
    {
        IRpsSetIss step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao)
            .SetServico(CodigoServico, ValorServicos);

        _ = Assert.IsAssignableFrom<IRpsSetIss>(step);
    }

    [Fact]
    public void SetServico_WithNullCodigoServico_ThrowsArgumentNullException()
    {
        CodigoServico? codigo = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, Tributacao)
                .SetServico(codigo!, ValorServicos));
    }

    [Fact]
    public void SetServico_WithNullValorServicos_ThrowsArgumentNullException()
    {
        Valor? valor = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, Tributacao)
                .SetServico(CodigoServico, valor!));
    }

    // ============================================
    // SetIss() Tests
    // ============================================

    [Fact]
    public void SetIss_WithValidParams_ReturnsIRpsSetOptionals()
    {
        IRpsSetOptionals step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao)
            .SetServico(CodigoServico, ValorServicos)
            .SetIss(Aliquota, false);

        _ = Assert.IsAssignableFrom<IRpsSetOptionals>(step);
    }

    [Fact]
    public void SetIss_WithNullAliquota_ThrowsArgumentNullException()
    {
        Aliquota? aliquota = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, Tributacao)
                .SetServico(CodigoServico, ValorServicos)
                .SetIss(aliquota!, false));
    }

    // ============================================
    // SetTomador() Tests
    // ============================================

    [Fact]
    public void SetTomador_WithValidTomador_ReturnsIRpsSetOptionals()
    {
        IRpsSetOptionals result = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao);

        _ = Assert.IsAssignableFrom<IRpsSetOptionals>(result);
    }

    [Fact]
    public void SetTomador_WithNullTomador_ThrowsArgumentNullException()
    {
        Tomador? tomador = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetTomador(tomador!));
    }

    // ============================================
    // SetIntermediario() Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void SetIntermediario_WithValidIntermediario_ReturnsIRpsSetOptionals(long cpfNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cpf)cpfNumber, true).Build();

        IRpsSetOptionals result = CadeiaObrigatoria(Serie).SetIntermediario(intermediario);

        _ = Assert.IsAssignableFrom<IRpsSetOptionals>(result);
    }

    [Fact]
    public void SetIntermediario_WithNullIntermediario_ThrowsArgumentNullException()
    {
        Intermediario? intermediario = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetIntermediario(intermediario!));
    }

    // ============================================
    // SetTributos() Tests
    // ============================================

    [Fact]
    public void SetTributos_WithAtLeastOneTributo_ReturnsIRpsSetOptionals()
    {
        IRpsSetOptionals result = CadeiaObrigatoria(Serie).SetTributos(valorPis: (Valor)10m);

        _ = Assert.IsAssignableFrom<IRpsSetOptionals>(result);
    }

    [Fact]
    public void SetTributos_WithAllValues_ReturnsIRpsSetOptionals()
    {
        IRpsSetOptionals result = CadeiaObrigatoria(Serie).SetTributos(
            valorPis: (Valor)10m,
            valorCofins: (Valor)10m,
            valorInss: (Valor)10m,
            valorIr: (Valor)10m,
            valorCsll: (Valor)10m);

        _ = Assert.IsAssignableFrom<IRpsSetOptionals>(result);
    }

    [Fact]
    public void SetTributos_WithAllNullValues_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(Serie).SetTributos());

    // ============================================
    // SetCargaTributaria() Tests
    // ============================================

    [Fact]
    public void SetCargaTributaria_WithValorOnly_ReturnsIRpsSetOptionals()
    {
        IRpsSetOptionals result = CadeiaObrigatoria(Serie).SetCargaTributaria(valorCargaTributaria: (Valor)500m);

        _ = Assert.IsAssignableFrom<IRpsSetOptionals>(result);
    }

    [Fact]
    public void SetCargaTributaria_WithAllNullValues_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(Serie).SetCargaTributaria());

    // ============================================
    // Optional Setters — Null Validation Tests
    // ============================================

    [Fact]
    public void SetCodigoCei_WithNull_ThrowsArgumentNullException()
    {
        Numero? cei = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetCodigoCei(cei!));
    }

    [Fact]
    public void SetObra_WithNull_ThrowsArgumentNullException()
    {
        Numero? obra = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetObra(obra!));
    }

    [Fact]
    public void SetMunicipio_WithNull_ThrowsArgumentNullException()
    {
        CodigoIbge? municipio = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetMunicipio(municipio!));
    }

    [Fact]
    public void SetEncapsulamento_WithNull_ThrowsArgumentNullException()
    {
        Numero? encapsulamento = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetEncapsulamento(encapsulamento!));
    }

    [Fact]
    public void SetValorRecebido_WithNull_ThrowsArgumentNullException()
    {
        Valor? valor = null;

        _ = Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetValorRecebido(valor!));
    }

    [Fact]
    public void SetInscricaoMunicipalPrestador_WithNull_ThrowsArgumentNullException()
    {
        InscricaoMunicipal? inscricao = null;

        _ = Assert.Throws<ArgumentNullException>(() =>
            CadeiaObrigatoria(Serie).SetInscricaoMunicipalPrestador(inscricao!));
    }

    // ============================================
    // Build() — Success Tests
    // ============================================

    [Fact]
    public void Build_WithMinimalRequiredChain_ReturnsRps()
    {
        Rps rps = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao).Build();

        Assert.NotNull(rps);
        Assert.NotNull(rps.ChaveRps);
        Assert.NotNull(rps.DataEmissao);
        Assert.NotNull(rps.ValorServicos);
        Assert.NotNull(rps.CodigoServico);
        Assert.NotNull(rps.AliquotaServicos);
        Assert.NotNull(rps.Discriminacao);
    }

    [Fact]
    public void Build_WithoutValorDeducoes_DefaultsToZero()
    {
        Rps rps = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao).Build();

        Assert.NotNull(rps.ValorDeducoes);
        Assert.Equal(((Valor)0m).ToString(), rps.ValorDeducoes.ToString());
    }

    [Fact]
    public void Build_WithValorDeducoes_PropagatesCorrectly()
    {
        var deducao = (Valor)5000m;
        Rps rps = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao)
            .SetServico(CodigoServico, ValorServicos, deducao)
            .SetIss(Aliquota, false)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.Equal(deducao, rps.ValorDeducoes);
    }

    [Fact]
    public void Build_WithIssRetidoTrue_PropagatesCorrectly()
    {
        Rps rps = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao)
            .SetServico(CodigoServico, ValorServicos)
            .SetIss(Aliquota, issRetido: true)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.True(rps.IssRetido);
    }

    [Fact]
    public void Build_WithStatusNfeNormal_PropagatesCorrectly()
    {
        Rps rps = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao).Build();

        Assert.Equal(StatusNfe.Normal, rps.StatusRps);
    }

    [Fact]
    public void Build_WithTributos_PropagatesAllValues()
    {
        Rps rps = CadeiaObrigatoria(Serie)
            .SetTributos(
                valorPis: (Valor)10m,
                valorCofins: (Valor)20m,
                valorInss: (Valor)30m,
                valorIr: (Valor)40m,
                valorCsll: (Valor)50m)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.Equal(((Valor)10m).ToString(), rps.ValorPis?.ToString());
        Assert.Equal(((Valor)20m).ToString(), rps.ValorCofins?.ToString());
        Assert.Equal(((Valor)30m).ToString(), rps.ValorInss?.ToString());
        Assert.Equal(((Valor)40m).ToString(), rps.ValorIr?.ToString());
        Assert.Equal(((Valor)50m).ToString(), rps.ValorCsll?.ToString());
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithIntermediario_PropagatesCorrectly(long cpfNumber)
    {
        Intermediario intermediario = IntermediarioBuilder.New((Cpf)cpfNumber, true).Build();

        Rps rps = CadeiaObrigatoria(Serie)
            .SetIntermediario(intermediario)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.NotNull(rps.CpfCnpjIntermediario);
        Assert.True(rps.IssRetidoIntermediario);
    }

    [Fact]
    public void Build_WithOptionalAddressFields_PropagatesCorrectly()
    {
        var municipio = new CodigoIbge(3550308);
        var codigoCei = new Numero(12345);
        var matriculaObra = new Numero(99999);

        Rps rps = CadeiaObrigatoria(Serie)
            .SetMunicipio(municipio)
            .SetCodigoCei(codigoCei)
            .SetObra(matriculaObra)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.NotNull(rps.MunicipioPrestacao);
        Assert.NotNull(rps.CodigoCei);
        Assert.NotNull(rps.MatriculaObra);
    }

    [Fact]
    public void Build_WithSetInscricaoMunicipalPrestador_OverridesOriginal()
    {
        var novaInscricao = new InscricaoMunicipal(12345678);

        Rps rps = CadeiaObrigatoria(Serie)
            .SetInscricaoMunicipalPrestador(novaInscricao)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.NotNull(rps.ChaveRps);
        Assert.Equal(novaInscricao.ToString(), rps.ChaveRps.InscricaoPrestador?.ToString());
    }

    [Fact]
    public void Build_WithCargaTributaria_PropagatesCorrectly()
    {
        var valor = (Valor)1500m;
        var percentual = (Percentual)0.15m;

        Rps rps = CadeiaObrigatoria(Serie)
            .SetCargaTributaria(valorCargaTributaria: valor, percentualCargaTributaria: percentual)
            .SetTomador(TomadorPadrao)
            .Build();

        Assert.Equal(valor, rps.ValorCargaTributaria);
        Assert.Equal(percentual, rps.PercentualCargaTributaria);
    }

    [Theory()]
    [ClassData(typeof(ValidCpfNumber))]
    public void Build_WithTomadorCpf_PropagatesCpfAndRazaoSocial(long cpfNumber)
    {
        Tomador tomador = TomadorBuilder
            .NewCpf((Cpf)cpfNumber)
            .SetEmail(new Email("tomador@teste.com.br"))
            .Build();

        Rps rps = CadeiaObrigatoria(Serie).SetTomador(tomador).Build();

        Assert.NotNull(rps.CpfOrCnpjTomador);
        Assert.Null(rps.RazaoSocialTomador);
        Assert.NotNull(rps.EmailTomador);
    }

    // ============================================
    // Build() — Failure Tests (runtime gaps do Step Builder)
    // ============================================

    [Fact]
    public void Build_WithoutSerieRps_ThrowsArgumentException() =>
        // serieRps=null é aceito por New(), mas rejeitado por Build()
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(serie: null).SetTomador(TomadorPadrao).Build());

    [Fact]
    public void Build_WithoutTomador_ThrowsArgumentException() =>
        // SetTomador() é opcional na cadeia mas obrigatório em Build()
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(Serie).Build());

    [Fact]
    public void Build_WithInvalidTributacaoNfeAndDataEmissao_ThrowsArgumentException()
    {
        // Arrange: 'A' is not valid before 2015-02-23
        var dataEmissao = new DataXsd(new DateTime(2015, 2, 22));
        var tributacao = (TributacaoNfe)'A';

        IRpsSetOptionals builder = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(dataEmissao, tributacao)
            .SetServico(CodigoServico, ValorServicos)
            .SetIss(Aliquota, false)
            .SetTomador(TomadorPadrao);

        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(() => builder.Build());
        Assert.Contains("T, F, I, J", ex.Message);
    }
}