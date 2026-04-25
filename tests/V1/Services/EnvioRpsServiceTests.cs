using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;

namespace Nfe.Paulistana.Tests.V1.Services;

/// <summary>
/// Testes unitários para <see cref="EnvioRpsService"/>:
/// guard clauses do construtor e de <see cref="IEnvioRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class EnvioRpsServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private PedidoEnvio CriarPedidoAssinado()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);
        return factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao());
    }

    private const string SoapEnvelopeVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="1"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioRPS></RetornoXML></EnvioRPSResponse></soap:Body></soap:Envelope>""";

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_HttpClientNulo_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new EnvioRpsService(null!));

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoEnvioNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio? pedidoEnvio = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedidoEnvio!));
    }

    [Fact]
    public async Task SendAsync_PedidoEnvioNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new EnvioRpsService(httpClient);
        var pedidoEnvio = new PedidoEnvio();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvio));
    }

    // ============================================
    // Resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_WebserviceRetornaRespostaSemPayload_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio pedidoEnvio = CriarPedidoAssinado();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvio));
    }

    [Fact]
    public async Task SendAsync_WebserviceRetornaRespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio pedidoEnvio = CriarPedidoAssinado();

        RetornoEnvioRps resultado = await service.SendAsync(pedidoEnvio);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_WebserviceRetornaRespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio pedidoEnvio = CriarPedidoAssinado();

        RetornoEnvioRps resultado = await service.SendAsync(pedidoEnvio);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }
}