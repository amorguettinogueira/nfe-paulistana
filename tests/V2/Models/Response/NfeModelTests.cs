using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Models.Response;

/// <summary>
/// Testes unitários para <see cref="NfeModel"/> (V2).
/// </summary>
public sealed class NfeModelTests
{
    private static string SerializarParaXml(NfeModel obj)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(NfeModel));
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, obj);
        return sw.ToString();
    }

    private static NfeModel? DesserializarDeXml(string xml)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(NfeModel));
        using var sr = new System.IO.StringReader(xml);
        return (NfeModel?)serializer.Deserialize(sr);
    }

    // ── Construção ────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new NfeModel());

    // ── Propriedades string (campos compartilhados com V1) ────────────────────

    [Fact]
    public void PropriedadesString_QuandoDefinidas_RetornamMesmosValores()
    {
        var nfe = new NfeModel
        {
            Assinatura = "assinatura-base64",
            NumeroLote = "1234",
            TipoRps = "RPS",
            DataEmissaoRps = "2024-01-15",
            RazaoSocialPrestador = "Empresa ABC Ltda",
            EmailPrestador = "contato@empresa.com",
            StatusNFe = "N",
            TributacaoNFe = "T",
            OpcaoSimples = "1",
            NumeroGuia = "9876",
            DataQuitacaoGuia = "2024-02-01",
            ValorServicos = "1500.00",
            ValorDeducoes = "0.00",
            ValorPis = "9.75",
            ValorCofins = "45.00",
            ValorInss = "0.00",
            ValorIr = "22.50",
            ValorCsll = "13.50",
            CodigoServico = "07498",
            AliquotaServicos = "5.00",
            ValorIss = "75.00",
            ValorCredito = "0.00",
            InscricaoMunicipalTomador = "00123456",
            InscricaoEstadualTomador = "123456789",
            RazaoSocialTomador = "Tomador XYZ",
            EmailTomador = "tomador@xyz.com",
            InscricaoMunicipalIntermediario = "00654321",
            IssRetidoIntermediario = "false",
            EmailIntermediario = "inter@xyz.com",
            Discriminacao = "Prestação de serviços de consultoria",
            ValorCargaTributaria = "300.00",
            PercentualCargaTributaria = "20.00",
            FonteCargaTributaria = "IBPT",
            CodigoCei = "12345678901",
            MatriculaObra = "111222",
            MunicipioPrestacao = "3550308",
            NumeroEncapsulamento = "55",
            ValorTotalRecebido = "1425.00"
        };

        Assert.Equal("assinatura-base64", nfe.Assinatura);
        Assert.Equal("1234", nfe.NumeroLote);
        Assert.Equal("RPS", nfe.TipoRps);
        Assert.Equal("2024-01-15", nfe.DataEmissaoRps);
        Assert.Equal("Empresa ABC Ltda", nfe.RazaoSocialPrestador);
        Assert.Equal("contato@empresa.com", nfe.EmailPrestador);
        Assert.Equal("N", nfe.StatusNFe);
        Assert.Equal("T", nfe.TributacaoNFe);
        Assert.Equal("1", nfe.OpcaoSimples);
        Assert.Equal("9876", nfe.NumeroGuia);
        Assert.Equal("2024-02-01", nfe.DataQuitacaoGuia);
        Assert.Equal("1500.00", nfe.ValorServicos);
        Assert.Equal("0.00", nfe.ValorDeducoes);
        Assert.Equal("9.75", nfe.ValorPis);
        Assert.Equal("45.00", nfe.ValorCofins);
        Assert.Equal("0.00", nfe.ValorInss);
        Assert.Equal("22.50", nfe.ValorIr);
        Assert.Equal("13.50", nfe.ValorCsll);
        Assert.Equal("07498", nfe.CodigoServico);
        Assert.Equal("5.00", nfe.AliquotaServicos);
        Assert.Equal("75.00", nfe.ValorIss);
        Assert.Equal("0.00", nfe.ValorCredito);
        Assert.Equal("00123456", nfe.InscricaoMunicipalTomador);
        Assert.Equal("123456789", nfe.InscricaoEstadualTomador);
        Assert.Equal("Tomador XYZ", nfe.RazaoSocialTomador);
        Assert.Equal("tomador@xyz.com", nfe.EmailTomador);
        Assert.Equal("00654321", nfe.InscricaoMunicipalIntermediario);
        Assert.Equal("false", nfe.IssRetidoIntermediario);
        Assert.Equal("inter@xyz.com", nfe.EmailIntermediario);
        Assert.Equal("Prestação de serviços de consultoria", nfe.Discriminacao);
        Assert.Equal("300.00", nfe.ValorCargaTributaria);
        Assert.Equal("20.00", nfe.PercentualCargaTributaria);
        Assert.Equal("IBPT", nfe.FonteCargaTributaria);
        Assert.Equal("12345678901", nfe.CodigoCei);
        Assert.Equal("111222", nfe.MatriculaObra);
        Assert.Equal("3550308", nfe.MunicipioPrestacao);
        Assert.Equal("55", nfe.NumeroEncapsulamento);
        Assert.Equal("1425.00", nfe.ValorTotalRecebido);
    }

    // ── Propriedades exclusivas da V2 ─────────────────────────────────────────

    [Fact]
    public void PropriedadesExclusivasV2_QuandoDefinidas_RetornamMesmosValores()
    {
        var nfe = new NfeModel
        {
            ValorInicialCobrado = "1500.00",
            ValorFinalCobrado = "1425.00",
            ValorMulta = "10.00",
            ValorJuros = "5.00",
            ValorIpi = "0.00",
            ExigibilidadeSuspensa = "1",
            PagamentoParceladoAntecipado = "0",
            Ncm = "85176290",
            Nbs = "1.0101.00.00"
        };

        Assert.Equal("1500.00", nfe.ValorInicialCobrado);
        Assert.Equal("1425.00", nfe.ValorFinalCobrado);
        Assert.Equal("10.00", nfe.ValorMulta);
        Assert.Equal("5.00", nfe.ValorJuros);
        Assert.Equal("0.00", nfe.ValorIpi);
        Assert.Equal("1", nfe.ExigibilidadeSuspensa);
        Assert.Equal("0", nfe.PagamentoParceladoAntecipado);
        Assert.Equal("85176290", nfe.Ncm);
        Assert.Equal("1.0101.00.00", nfe.Nbs);
    }

    // ── Propriedades booleanas e de data ──────────────────────────────────────

    [Fact]
    public void IssRetido_QuandoDefinidoComoTrue_RetornaTrue()
    {
        var nfe = new NfeModel { IssRetido = true };

        Assert.True(nfe.IssRetido);
    }

    [Fact]
    public void IssRetido_QuandoDefinidoComoFalse_RetornaFalse()
    {
        var nfe = new NfeModel { IssRetido = false };

        Assert.False(nfe.IssRetido);
    }

    [Fact]
    public void DataEmissaoNFe_QuandoDefinida_RetornaMesmoValor()
    {
        var data = new DateTime(2024, 6, 15, 10, 30, 0);
        var nfe = new NfeModel { DataEmissaoNFe = data };

        Assert.Equal(data, nfe.DataEmissaoNFe);
    }

    [Fact]
    public void DataFatoGeradorNFe_QuandoDefinida_RetornaMesmoValor()
    {
        var data = new DateTime(2024, 6, 15);
        var nfe = new NfeModel { DataFatoGeradorNFe = data };

        Assert.Equal(data, nfe.DataFatoGeradorNFe);
    }

    // ── Propriedades tipadas ──────────────────────────────────────────────────

    [Fact]
    public void ChaveNFe_QuandoDefinida_RetornaMesmaReferencia()
    {
        var chave = new ChaveNFe { NumeroNFe = "0001234" };
        var nfe = new NfeModel { ChaveNFe = chave };

        Assert.Same(chave, nfe.ChaveNFe);
    }

    [Fact]
    public void ChaveRps_QuandoDefinido_RetornaMesmaReferencia()
    {
        var chave = new ChaveRps { NumeroRps = "42" };
        var nfe = new NfeModel { ChaveRps = chave };

        Assert.Same(chave, nfe.ChaveRps);
    }

    [Fact]
    public void CpfCnpjPrestador_QuandoDefinido_RetornaMesmaReferencia()
    {
        var doc = new CpfCnpj { Cpf = "46381819618" };
        var nfe = new NfeModel { CpfCnpjPrestador = doc };

        Assert.Same(doc, nfe.CpfCnpjPrestador);
    }

    [Fact]
    public void EnderecoPrestador_QuandoDefinido_RetornaMesmaReferencia()
    {
        var endereco = new EnderecoNfe { Logradouro = "Av. Paulista" };
        var nfe = new NfeModel { EnderecoPrestador = endereco };

        Assert.Same(endereco, nfe.EnderecoPrestador);
    }

    [Fact]
    public void CpfCnpjTomador_QuandoDefinido_RetornaMesmaReferencia()
    {
        var doc = new CpfCnpj { Cnpj = "00878130000121" };
        var nfe = new NfeModel { CpfCnpjTomador = doc };

        Assert.Same(doc, nfe.CpfCnpjTomador);
    }

    [Fact]
    public void EnderecoTomador_QuandoDefinido_RetornaMesmaReferencia()
    {
        var endereco = new EnderecoNfe { Logradouro = "Rua das Flores" };
        var nfe = new NfeModel { EnderecoTomador = endereco };

        Assert.Same(endereco, nfe.EnderecoTomador);
    }

    [Fact]
    public void CpfCnpjIntermediario_QuandoDefinido_RetornaMesmaReferencia()
    {
        var doc = new CpfCnpj { Cpf = "46381819618" };
        var nfe = new NfeModel { CpfCnpjIntermediario = doc };

        Assert.Same(doc, nfe.CpfCnpjIntermediario);
    }

    // ── Valores padrão ────────────────────────────────────────────────────────

    [Fact]
    public void Propriedades_QuandoNaoDefinidas_RetornamValoresPadrao()
    {
        var nfe = new NfeModel();

        Assert.Null(nfe.Assinatura);
        Assert.Null(nfe.ChaveNFe);
        Assert.Equal(default, nfe.DataEmissaoNFe);
        Assert.Null(nfe.NumeroLote);
        Assert.Null(nfe.ChaveRps);
        Assert.Null(nfe.TipoRps);
        Assert.Null(nfe.DataEmissaoRps);
        Assert.Equal(default, nfe.DataFatoGeradorNFe);
        Assert.Null(nfe.CpfCnpjPrestador);
        Assert.Null(nfe.RazaoSocialPrestador);
        Assert.Null(nfe.EnderecoPrestador);
        Assert.Null(nfe.EmailPrestador);
        Assert.Null(nfe.StatusNFe);
        Assert.Null(nfe.DataCancelamento);
        Assert.Null(nfe.TributacaoNFe);
        Assert.Null(nfe.OpcaoSimples);
        Assert.Null(nfe.ValorServicos);
        Assert.False(nfe.IssRetido);
        Assert.Null(nfe.CpfCnpjTomador);
        Assert.Null(nfe.EnderecoTomador);
        Assert.Null(nfe.CpfCnpjIntermediario);
        Assert.Null(nfe.Discriminacao);
        Assert.Null(nfe.ValorTotalRecebido);
        Assert.Null(nfe.ValorInicialCobrado);
        Assert.Null(nfe.ValorFinalCobrado);
        Assert.Null(nfe.ValorMulta);
        Assert.Null(nfe.ValorJuros);
        Assert.Null(nfe.ValorIpi);
        Assert.Null(nfe.ExigibilidadeSuspensa);
        Assert.Null(nfe.PagamentoParceladoAntecipado);
        Assert.Null(nfe.Ncm);
        Assert.Null(nfe.Nbs);
    }

    // ── Serialização XML — round-trip ─────────────────────────────────────────

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaPropriedades()
    {
        // Arrange
        var data = new DateTime(2024, 6, 15, 10, 0, 0);
        var nfe = new NfeModel
        {
            ChaveNFe = new ChaveNFe { InscricaoPrestador = "12345678", NumeroNFe = "0001234", CodigoVerificacao = "ABCDEFGH" },
            DataEmissaoNFe = data,
            DataFatoGeradorNFe = data,
            RazaoSocialPrestador = "Empresa Teste",
            StatusNFe = "N",
            ValorServicos = "1000.00",
            ValorIss = "50.00",
            IssRetido = false,
            Discriminacao = "Consultoria técnica",
            ValorInicialCobrado = "1000.00",
            ValorFinalCobrado = "950.00",
            Ncm = "85176290",
            Nbs = "1.0101.00.00"
        };

        // Act
        var desserializado = DesserializarDeXml(SerializarParaXml(nfe))!;

        // Assert
        Assert.Equal(nfe.ChaveNFe.NumeroNFe, desserializado.ChaveNFe!.NumeroNFe);
        Assert.Equal(nfe.DataEmissaoNFe, desserializado.DataEmissaoNFe);
        Assert.Equal(nfe.RazaoSocialPrestador, desserializado.RazaoSocialPrestador);
        Assert.Equal(nfe.StatusNFe, desserializado.StatusNFe);
        Assert.Equal(nfe.ValorServicos, desserializado.ValorServicos);
        Assert.Equal(nfe.ValorIss, desserializado.ValorIss);
        Assert.Equal(nfe.IssRetido, desserializado.IssRetido);
        Assert.Equal(nfe.Discriminacao, desserializado.Discriminacao);
        Assert.Equal(nfe.ValorInicialCobrado, desserializado.ValorInicialCobrado);
        Assert.Equal(nfe.ValorFinalCobrado, desserializado.ValorFinalCobrado);
        Assert.Equal(nfe.Ncm, desserializado.Ncm);
        Assert.Equal(nfe.Nbs, desserializado.Nbs);
    }

    // ── Serialização XML — nomes dos elementos ────────────────────────────────

    [Fact]
    public void XmlSerialization_UsaNomesCorretosDosElementosCompartilhados()
    {
        // Arrange
        var nfe = new NfeModel
        {
            ChaveNFe = new ChaveNFe { NumeroNFe = "0001234" },
            ChaveRps = new ChaveRps { NumeroRps = "1" },
            TipoRps = "RPS",
            DataEmissaoRps = "2024-01-15",
            CpfCnpjPrestador = new CpfCnpj { Cpf = "46381819618" },
            ValorPis = "9.75",
            ValorCofins = "45.00",
            ValorInss = "0.00",
            ValorIr = "22.50",
            ValorCsll = "13.50",
            ValorIss = "75.00",
            IssRetido = true,
            CpfCnpjTomador = new CpfCnpj { Cnpj = "00878130000121" },
            CpfCnpjIntermediario = new CpfCnpj { Cpf = "46381819618" },
            CodigoCei = "12345678901"
        };

        // Act
        var xml = SerializarParaXml(nfe);

        // Assert
        Assert.Contains("<ChaveNFe>", xml);
        Assert.Contains("<ChaveRPS>", xml);
        Assert.Contains("<TipoRPS>RPS</TipoRPS>", xml);
        Assert.Contains("<DataEmissaoRPS>2024-01-15</DataEmissaoRPS>", xml);
        Assert.Contains("<CPFCNPJPrestador>", xml);
        Assert.Contains("<ValorPIS>9.75</ValorPIS>", xml);
        Assert.Contains("<ValorCOFINS>45.00</ValorCOFINS>", xml);
        Assert.Contains("<ValorINSS>0.00</ValorINSS>", xml);
        Assert.Contains("<ValorIR>22.50</ValorIR>", xml);
        Assert.Contains("<ValorCSLL>13.50</ValorCSLL>", xml);
        Assert.Contains("<ValorISS>75.00</ValorISS>", xml);
        Assert.Contains("<ISSRetido>true</ISSRetido>", xml);
        Assert.Contains("<CPFCNPJTomador>", xml);
        Assert.Contains("<CPFCNPJIntermediario>", xml);
        Assert.Contains("<CodigoCEI>12345678901</CodigoCEI>", xml);
    }

    [Fact]
    public void XmlSerialization_UsaNomesCorretosDosElementosExclusivosV2()
    {
        // Arrange
        var nfe = new NfeModel
        {
            ValorInicialCobrado = "1500.00",
            ValorFinalCobrado = "1425.00",
            ValorMulta = "10.00",
            ValorJuros = "5.00",
            ValorIpi = "0.00",
            Ncm = "85176290",
            Nbs = "1.0101.00.00"
        };

        // Act
        var xml = SerializarParaXml(nfe);

        // Assert
        Assert.Contains("<ValorInicialCobrado>1500.00</ValorInicialCobrado>", xml);
        Assert.Contains("<ValorFinalCobrado>1425.00</ValorFinalCobrado>", xml);
        Assert.Contains("<ValorMulta>10.00</ValorMulta>", xml);
        Assert.Contains("<ValorJuros>5.00</ValorJuros>", xml);
        Assert.Contains("<ValorIPI>0.00</ValorIPI>", xml);
        Assert.Contains("<NCM>85176290</NCM>", xml);
        Assert.Contains("<NBS>1.0101.00.00</NBS>", xml);
    }
}
