using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Builders;

public class RpsBuilderTests
{
    private static readonly InscricaoMunicipal InscricaoPrestador = new(39616924);
    private static readonly Numero NumeroRps = new(4105);
    private static readonly Discriminacao Discriminacao = new("Desenvolvimento de software.");
    private static readonly SerieRps Serie = new("BB");
    private static readonly DataXsd DataEmissao = new(new DateTime(2024, 1, 20));
    private static readonly TributacaoNfe Tributacao = (TributacaoNfe)'T';
    private static readonly CodigoServico CodigoServico = new(7617);
    private static readonly CodigoNBS CodigoNbs = new("123456789");
    private static readonly Aliquota Aliquota = (Aliquota)0.05m;
    private static readonly InformacoesIbsCbs IbsCbs = new();

    private static readonly Tomador TomadorPadrao =
        TomadorBuilder.NewCpf(new Cpf(46381819618)).Build();

    private static IRpsSetOptionals CadeiaObrigatoria(SerieRps? serie = null) =>
        RpsBuilder.New(InscricaoPrestador, TipoRps.NotaFiscalConjugada, NumeroRps, Discriminacao, serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs)
            .SetIss(Aliquota, false)
            .SetIbsCbs(IbsCbs)
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)1234567);

    [Fact]
    public void New_WithAllRequiredParams_ReturnsIRpsSetNfe()
    {
        var step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie);
        Assert.IsAssignableFrom<IRpsSetNfe>(step);
    }

    [Fact]
    public void New_WithNullInscricaoPrestador_ThrowsArgumentNullException()
    {
        InscricaoMunicipal? inscricao = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(inscricao!, TipoRps.Rps, NumeroRps, Discriminacao, Serie));
    }

    [Fact]
    public void New_WithNullNumeroRps_ThrowsArgumentNullException()
    {
        Numero? numero = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, numero!, Discriminacao, Serie));
    }

    [Fact]
    public void New_WithNullDiscriminacao_ThrowsArgumentNullException()
    {
        Discriminacao? discriminacao = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, discriminacao!, Serie));
    }

    [Fact]
    public void New_WithNullSerieRps_ReturnsIRpsSetNfe()
    {
        var step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, null);
        Assert.IsAssignableFrom<IRpsSetNfe>(step);
    }

    [Fact]
    public void SetNFe_WithValidParams_ReturnsIRpsSetServico()
    {
        var step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false));
        Assert.IsAssignableFrom<IRpsSetServico>(step);
    }

    [Fact]
    public void SetNFe_WithNullDataEmissao_ThrowsArgumentNullException()
    {
        DataXsd? dataEmissao = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(dataEmissao!, Tributacao, new NaoSim(false), new NaoSim(false)));
    }

    [Fact]
    public void SetNFe_WithNullTributacao_ThrowsArgumentNullException()
    {
        TributacaoNfe? tributacao = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, tributacao!, new NaoSim(false), new NaoSim(false)));
    }

    [Fact]
    public void SetServico_WithValidParams_ReturnsIRpsSetIss()
    {
        var step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs);
        Assert.IsAssignableFrom<IRpsSetIss>(step);
    }

    [Fact]
    public void SetServico_WithNullCodigoServico_ThrowsArgumentNullException()
    {
        CodigoServico? codigo = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
                .SetServico(codigo!, CodigoNbs));
    }

    [Fact]
    public void SetServico_WithNullNbs_ThrowsArgumentNullException()
    {
        CodigoNBS? nbs = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
                .SetServico(CodigoServico, nbs!));
    }

    [Fact]
    public void SetIss_WithValidParams_ReturnsIRpsSetIbsCbs()
    {
        var step = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs)
            .SetIss(Aliquota, false);
        Assert.IsAssignableFrom<IRpsSetIbsCbs>(step);
    }

    [Fact]
    public void SetIss_WithNullAliquota_ThrowsArgumentNullException()
    {
        Aliquota? aliquota = null;
        Assert.Throws<ArgumentNullException>(() =>
            RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
                .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
                .SetServico(CodigoServico, CodigoNbs)
                .SetIss(aliquota!, false));
    }

    [Fact]
    public void SetIbsCbs_WithValidIbsCbs_ReturnsIRpsSetOptionals()
    {
        var builder = RpsBuilder.New(InscricaoPrestador, TipoRps.NotaFiscalConjugada, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs)
            .SetIss(Aliquota, false);
        var step = builder.SetIbsCbs(IbsCbs);
        Assert.IsAssignableFrom<IRpsSetOptionals>(step);
    }

    [Fact]
    public void SetIbsCbs_WithNullIbsCbs_ThrowsArgumentNullException()
    {
        InformacoesIbsCbs? ibsCbs = null;
        var builder = RpsBuilder.New(InscricaoPrestador, TipoRps.NotaFiscalConjugada, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs)
            .SetIss(Aliquota, false);
        Assert.Throws<ArgumentNullException>(() => builder.SetIbsCbs(ibsCbs!));
    }

    [Fact]
    public void SetTomador_WithValidTomador_ReturnsIRpsSetOptionals()
    {
        var result = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao);
        Assert.IsAssignableFrom<IRpsSetOptionals>(result);
    }

    [Fact]
    public void SetTomador_WithNullTomador_ThrowsArgumentNullException()
    {
        Tomador? tomador = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetTomador(tomador!));
    }

    [Fact]
    public void SetIntermediario_WithNullIntermediario_ThrowsArgumentNullException()
    {
        Intermediario? intermediario = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetIntermediario(intermediario!));
    }

    [Fact]
    public void SetTributos_WithAllNullValues_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(Serie).SetTributos());

    [Fact]
    public void SetCargaTributaria_WithAllNullValues_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(Serie).SetCargaTributaria());

    [Fact]
    public void SetCodigoCei_WithNull_ThrowsArgumentNullException()
    {
        Numero? cei = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetCodigoCei(cei!));
    }

    [Fact]
    public void SetObra_WithNull_ThrowsArgumentNullException()
    {
        Numero? obra = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetObra(obra!));
    }

    [Fact]
    public void SetMunicipio_WithNull_ThrowsArgumentNullException()
    {
        CodigoIbge? municipio = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetMunicipio(municipio!));
    }

    [Fact]
    public void SetEncapsulamento_WithNull_ThrowsArgumentNullException()
    {
        Numero? encapsulamento = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetEncapsulamento(encapsulamento!));
    }

    [Fact]
    public void SetValorRecebido_WithNull_ThrowsArgumentNullException()
    {
        Valor? valor = null;
        Assert.Throws<ArgumentNullException>(() => CadeiaObrigatoria(Serie).SetValorRecebido(valor!));
    }

    [Fact]
    public void SetInscricaoMunicipalPrestador_WithNull_ThrowsArgumentNullException()
    {
        InscricaoMunicipal? inscricao = null;
        Assert.Throws<ArgumentNullException>(() =>
            CadeiaObrigatoria(Serie).SetInscricaoMunicipalPrestador(inscricao!));
    }

    [Fact]
    public void Build_WithMinimalRequiredChain_ReturnsRps()
    {
        var rps = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao).Build();
        Assert.NotNull(rps);
        Assert.NotNull(rps.ChaveRps);
        Assert.NotNull(rps.DataEmissao);
        Assert.NotNull(rps.CodigoServico);
        Assert.NotNull(rps.AliquotaServicos);
        Assert.NotNull(rps.Discriminacao);
    }

    [Fact]
    public void Build_WithoutValorDeducoes_DefaultsToZero()
    {
        var rps = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao).Build();
        Assert.NotNull(rps.ValorDeducoes);
        Assert.Equal(((Valor)0).ToString(), rps.ValorDeducoes.ToString());
    }

    [Fact]
    public void Build_WithValorDeducoes_PropagatesCorrectly()
    {
        var deducao = (Valor)5000m;
        var rps = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs, deducao)
            .SetIss(Aliquota, false)
            .SetIbsCbs(IbsCbs)
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)1234567)
            .SetTomador(TomadorPadrao)
            .Build();
        Assert.Equal(deducao, rps.ValorDeducoes);
    }

    [Fact]
    public void Build_WithIssRetidoTrue_PropagatesCorrectly()
    {
        var rps = RpsBuilder.New(InscricaoPrestador, TipoRps.Rps, NumeroRps, Discriminacao, Serie)
            .SetNFe(DataEmissao, Tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(CodigoServico, CodigoNbs)
            .SetIss(Aliquota, true)
            .SetIbsCbs(IbsCbs)
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)1234567)
            .SetTomador(TomadorPadrao)
            .Build();
        Assert.True(rps.IssRetido);
    }

    [Fact]
    public void Build_WithStatusNfeNormal_PropagatesCorrectly()
    {
        var rps = CadeiaObrigatoria(Serie).SetTomador(TomadorPadrao).Build();
        Assert.Equal(StatusNfe.Normal, rps.StatusRps);
    }

    [Fact]
    public void Build_WithTributos_PropagatesAllValues()
    {
        var rps = CadeiaObrigatoria(Serie)
            .SetTributos(
                valorPis: (Valor)10m,
                valorCofins: (Valor)20m,
                valorInss: (Valor)30m,
                valorIr: (Valor)40m,
                valorCsll: (Valor)50m,
                valorIpi: (Valor)60m)
            .SetTomador(TomadorPadrao)
            .Build();
        Assert.Equal(((Valor)10m).ToString(), rps.ValorPis?.ToString());
        Assert.Equal(((Valor)20m).ToString(), rps.ValorCofins?.ToString());
        Assert.Equal(((Valor)30m).ToString(), rps.ValorInss?.ToString());
        Assert.Equal(((Valor)40m).ToString(), rps.ValorIr?.ToString());
        Assert.Equal(((Valor)50m).ToString(), rps.ValorCsll?.ToString());
        Assert.Equal(((Valor)60m).ToString(), rps.ValorIpi?.ToString());
    }

    [Fact]
    public void Build_WithIntermediario_PropagatesCorrectly()
    {
        var intermediario = IntermediarioBuilder.New(new Cpf(46381819618), true).Build();
        var rps = CadeiaObrigatoria(Serie)
            .SetIntermediario(intermediario)
            .SetTomador(TomadorPadrao)
            .Build();
        Assert.NotNull(rps.CpfCnpjIntermediario);
        Assert.True(rps.IssRetidoIntermediario ?? false);
    }

    [Fact]
    public void Build_WithOptionalAddressFields_PropagatesCorrectly()
    {
        var municipio = new CodigoIbge(3550308);
        var codigoCei = new Numero(12345);
        var matriculaObra = new Numero(99999);
        var rps = CadeiaObrigatoria(Serie)
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
        var rps = CadeiaObrigatoria(Serie)
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
        var rps = CadeiaObrigatoria(Serie)
            .SetCargaTributaria(valorCargaTributaria: valor, percentualCargaTributaria: percentual)
            .SetTomador(TomadorPadrao)
            .Build();
        Assert.Equal(valor, rps.ValorCargaTributaria);
        Assert.Equal(percentual, rps.PercentualCargaTributaria);
    }

    [Fact]
    public void Build_WithTomadorCpf_PropagatesCpfAndRazaoSocial()
    {
        var tomador = TomadorBuilder
            .NewCpf(new Cpf(46381819618))
            .SetEmail(new Email("tomador@teste.com.br"))
            .Build();
        var rps = CadeiaObrigatoria(Serie).SetTomador(tomador).Build();
        Assert.NotNull(rps.CpfCnpjNifTomador);
        Assert.Null(rps.RazaoSocialTomador);
        Assert.NotNull(rps.EmailTomador);
    }

    [Fact]
    public void Build_WithoutSerieRps_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(serie: null).SetTomador(TomadorPadrao).Build());

    [Fact]
    public void Build_WithoutTomador_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadeiaObrigatoria(Serie).Build());
}