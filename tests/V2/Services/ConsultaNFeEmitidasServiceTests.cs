using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="ConsultaNFeEmitidasService"/> v02:
/// guard clauses do construtor e de <see cref="IConsultaNFeEmitidasService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class ConsultaNFeEmitidasServiceTests
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
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaNFeEmitidasResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload (Versao=2)
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaNFeEmitidasResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoConsulta xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho><NFe xmlns=""><ChaveNFe><InscricaoPrestador>39616924</InscricaoPrestador><NumeroNFe>2000</NumeroNFe></ChaveNFe><DataEmissaoNFe>2024-03-10T09:00:00</DataEmissaoNFe><DataFatoGeradorNFe>2024-03-10T00:00:00</DataFatoGeradorNFe><StatusNFe>N</StatusNFe><ISSRetido>false</ISSRetido></NFe></RetornoConsulta></RetornoXML></ConsultaNFeEmitidasResponse></soap:Body></soap:Envelope>""";

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
        _ = Assert.Throws<ArgumentNullException>(() => new ConsultaNFeEmitidasService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo? pedido = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedido!));
    }

    [Fact]
    public async Task SendAsync_PedidoNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new ConsultaNFeEmitidasService(httpClient);
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
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    [Fact]
    public async Task SendAsync_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoVersaoIgualADois()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.Equal(2, resultado.Cabecalho?.Versao);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_NfesContemUmaEntrada()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado.Nfes);
        Assert.Single(resultado.Nfes);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_NfeContemChaveNFe()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeEmitidasService(httpClient);
        PedidoConsultaNFePeriodo pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado.Nfes?[0].ChaveNFe);
    }
}
