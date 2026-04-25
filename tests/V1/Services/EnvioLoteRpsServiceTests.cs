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
/// Testes unitários para <see cref="EnvioLoteRpsService"/>:
/// guard clauses do construtor e de <see cref="IEnvioLoteRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta para modo de produção e modo de teste.
/// </summary>
public class EnvioLoteRpsServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private PedidoEnvioLote CriarLoteAssinado()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        return factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao()]);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote? pedidoEnvioLote = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedidoEnvioLote!));
    }

    [Fact]
    public async Task SendAsync_PedidoEnvioLoteNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote, modoTeste: false));
    }

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: false);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_ModoProducao_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteVazio);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvioLote, modoTeste: true));
    }

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_ModoTeste_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }
}