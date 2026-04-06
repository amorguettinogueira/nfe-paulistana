using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Services;

/// <summary>
/// Testes unitários para <see cref="ConsultaNFeRecebidasService"/>:
/// guard clauses do construtor e de <see cref="IConsultaNFeRecebidasService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class ConsultaNFeRecebidasServiceTests
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

    private static PedidoConsultaNFePeriodo CriarConsultaAssinada()
    {
        var factory = new PedidoConsultaNFePeriodoFactory(CriarConfiguracao());
        var cpf = new Cpf(new ValidCpfNumber().Min());
        var cpfCnpj = new CpfOrCnpj(cpf);
        var dtInicio = new DataXsd(new DateTime(2024, 1, 1));
        var dtFim = new DataXsd(new DateTime(2024, 12, 31));
        var numeroPagina = new Numero(1);
        return factory.NewCpf(cpf, cpfCnpj, null, dtInicio, dtFim, numeroPagina);
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

    // Sem payload
    private const string SoapEnvelopeVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaNFeRecebidasResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaNFeRecebidasResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoConsulta xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="1"><Sucesso>true</Sucesso></Cabecalho></RetornoConsulta></RetornoXML></ConsultaNFeRecebidasResponse></soap:Body></soap:Envelope>""";

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
        _ = Assert.Throws<ArgumentNullException>(() => new ConsultaNFeRecebidasService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaNFeRecebidasService(httpClient);
        PedidoConsultaNFePeriodo? pedido = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedido!));
    }

    [Fact]
    public async Task SendAsync_PedidoNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaNFeRecebidasService(httpClient);
        var pedido = new PedidoConsultaNFePeriodo();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    // ============================================
    // Resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaNFeRecebidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    [Fact]
    public async Task SendAsync_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeRecebidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeRecebidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }
}
