using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Services;

/// <summary>
/// Testes unitários para <see cref="EnvioLoteRpsService"/>:
/// guard clauses do construtor e de <see cref="IEnvioLoteRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta para modo de produção e modo de teste.
/// </summary>
public class EnvioLoteRpsServiceTests
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

    private static Rps CriarRps() =>
        RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(4105),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), (Valor)1000m)
            .SetIss((Aliquota)0.05m, false)
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
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioLoteRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="1"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioLoteRPS></RetornoXML></EnvioLoteRPSResponse></soap:Body></soap:Envelope>""";

    // Teste — sem payload
    private const string SoapEnvelopeTesteVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><TesteEnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Teste — com payload
    private const string SoapEnvelopeTesteComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><TesteEnvioLoteRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioLoteRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="1"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioLoteRPS></RetornoXML></TesteEnvioLoteRPSResponse></soap:Body></soap:Envelope>""";

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
        _ = Assert.Throws<ArgumentNullException>(() => new EnvioLoteRpsService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoEnvioLoteNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote? pedidoEnvioLote = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedidoEnvioLote!));
    }

    [Fact]
    public async Task SendAsync_PedidoEnvioLoteNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        var pedidoEnvioLote = new PedidoEnvioLote();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote));
    }

    // ============================================
    // Modo de produção — resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote, modoTeste: false));
    }

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: false);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: false);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    // ============================================
    // Modo de teste — resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote, modoTeste: true));
    }

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }
}