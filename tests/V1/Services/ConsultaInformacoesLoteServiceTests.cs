using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Services;

/// <summary>
/// Testes unitários para <see cref="ConsultaInformacoesLoteService"/>:
/// guard clauses do construtor e de <see cref="IConsultaInformacoesLoteService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class ConsultaInformacoesLoteServiceTests
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

    private static PedidoInformacoesLote CriarConsultaAssinada()
    {
        var factory = new PedidoInformacoesLoteFactory(CriarConfiguracao());
        var cpf = new Cpf(new ValidCpfNumber().Min());
        var inscricao = new InscricaoMunicipal(39616924);
        var numeroLote = new Numero(12345);
        return factory.NewCpf(cpf, inscricao, numeroLote);
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
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaInformacoesLoteResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaInformacoesLoteResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoInformacoesLote xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="1"><Sucesso>true</Sucesso><InformacoesLote><NumeroLote>12345</NumeroLote><InscricaoPrestador>39616924</InscricaoPrestador><DataEnvioLote>2024-01-15T10:30:00</DataEnvioLote><QtdNotasProcessadas>5</QtdNotasProcessadas><TempoProcessamento>1200</TempoProcessamento><ValorTotalServicos>5000.00</ValorTotalServicos></InformacoesLote></Cabecalho></RetornoInformacoesLote></RetornoXML></ConsultaInformacoesLoteResponse></soap:Body></soap:Envelope>""";

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
        _ = Assert.Throws<ArgumentNullException>(() => new ConsultaInformacoesLoteService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaInformacoesLoteService(httpClient);
        PedidoInformacoesLote? pedido = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedido!));
    }

    [Fact]
    public async Task SendAsync_PedidoNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaInformacoesLoteService(httpClient);
        var pedido = new PedidoInformacoesLote();

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
        var service = new ConsultaInformacoesLoteService(httpClient);
        PedidoInformacoesLote pedido = CriarConsultaAssinada();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    [Fact]
    public async Task SendAsync_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaInformacoesLoteService(httpClient);
        PedidoInformacoesLote pedido = CriarConsultaAssinada();

        RetornoInformacoesLote resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaInformacoesLoteService(httpClient);
        PedidoInformacoesLote pedido = CriarConsultaAssinada();

        RetornoInformacoesLote resultado = await service.SendAsync(pedido);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_InformacoesLoteContemDados()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaInformacoesLoteService(httpClient);
        PedidoInformacoesLote pedido = CriarConsultaAssinada();

        RetornoInformacoesLote resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado.Cabecalho?.InformacoesLote);
        Assert.Equal("12345", resultado.Cabecalho.InformacoesLote.NumeroLote);
        Assert.Equal(5, resultado.Cabecalho.InformacoesLote.QtdNotasProcessadas);
        Assert.Equal(5000.00m, resultado.Cabecalho.InformacoesLote.ValorTotalServicos);
    }
}