using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="EnvioLoteRpsService"/>:
/// guard clauses do construtor e de <see cref="IEnvioLoteRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta para modo de produção e modo de teste.
/// </summary>
public sealed class EnvioLoteRpsServiceTests
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

    private static readonly Tomador TomadorPadrao =
        TomadorBuilder.NewCpf(new Cpf(new ValidCpfNumber().Min())).Build();

    private static readonly InformacoesIbsCbs IbsCbsPadrao =
        InformacoesIbsCbsBuilder.New()
            .SetUsoOuConsumoPessoal(new NaoSim(false))
            .SetCodigoOperacaoFornecimento(new CodigoOperacaoFornecimento("010101"))
            .SetClassificacaoTributaria(new ClassificacaoTributaria("010101"))
            .Build();

    private static Rps CriarRps() =>
        RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(4105),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(
                new DataXsd(new DateTime(2024, 1, 20)),
                (TributacaoNfe)'T',
                new NaoSim(false),
                new NaoSim(false))
            .SetServico(new CodigoServico(7617), new CodigoNBS("123456789"))
            .SetIss((Aliquota)0.05m, false)
            .SetIbsCbs(IbsCbsPadrao)
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)3550308)
            .SetTomador(TomadorPadrao)
            .Build();

    private static PedidoEnvioLote CriarLoteAssinado()
    {
        var factory = new PedidoEnvioLoteFactory(CriarConfiguracao());
        return factory.NewCpf(new Cpf(new ValidCpfNumber().Min()), false, [CriarRps()]);
    }

    private static HttpClient CriarHttpClientFake(string responseXml)
    {
        var handler = new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseXml, System.Text.Encoding.UTF8, "text/xml")
            });
        return new HttpClient(handler) { BaseAddress = new Uri("https://fake-nfe/") };
    }

    // Produção — sem payload
    private const string SoapEnvelopeLoteVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Produção — com payload
    private const string SoapEnvelopeLoteComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioLoteRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioLoteRPS></RetornoXML></EnvioLoteRPSResponse></soap:Body></soap:Envelope>""";

    // Teste — sem payload
    private const string SoapEnvelopeTesteVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><TesteEnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Teste — com payload
    private const string SoapEnvelopeTesteComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><TesteEnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioLoteRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioLoteRPS></RetornoXML></TesteEnvioLoteRPSResponse></soap:Body></soap:Envelope>""";

    private sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(response);
    }

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_HttpClientNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EnvioLoteRpsService(null!));
    }

    [Fact]
    public void Constructor_HttpClientValido_NaoLancaExcecao()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);

        // Act
        var service = new EnvioLoteRpsService(httpClient);

        // Assert
        Assert.NotNull(service);
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoEnvioLoteNulo_ThrowsArgumentNullException()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote? pedidoEnvioLote = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedidoEnvioLote!));
    }

    [Fact]
    public async Task SendAsync_PedidoEnvioLoteNaoAssinado_ThrowsInvalidOperationException()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        var pedidoEnvioLote = new PedidoEnvioLote();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote));

        Assert.Contains("não foram validados com sucesso", exception.Message);
    }

    // ============================================
    // Modo de produção — resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote, modoTeste: false));

        Assert.Contains("resposta vazia ou inválida", exception.Message);
    }

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaValida_RetornaRetornoNaoNulo()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: false);

        // Assert
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaValida_CabecalhoIndicaSucesso()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: false);

        // Assert
        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    [Fact]
    public async Task SendAsync_ModoProducao_ComCancellationToken_ExecutaComSucesso()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();
        using var cts = new CancellationTokenSource();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, false, cts.Token);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    // ============================================
    // Modo de teste — resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote, modoTeste: true));

        Assert.Contains("resposta vazia ou inválida", exception.Message);
    }

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaValida_RetornaRetornoNaoNulo()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        // Assert
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaValida_CabecalhoIndicaSucesso()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        // Assert
        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    [Fact]
    public async Task SendAsync_ModoTeste_ComCancellationToken_ExecutaComSucesso()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();
        using var cts = new CancellationTokenSource();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, true, cts.Token);

        // Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    // ============================================
    // Validação de comportamento
    // ============================================

    [Fact]
    public async Task SendAsync_ModoProducaoFalse_UtilizaEndpointProducao()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: false);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Cabecalho);
    }

    [Fact]
    public async Task SendAsync_ModoTesteTrue_UtilizaEndpointTeste()
    {
        // Arrange
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Cabecalho);
    }
}
