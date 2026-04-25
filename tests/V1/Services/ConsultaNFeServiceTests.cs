using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;
using DomainChaveRps = Nfe.Paulistana.V1.Models.Domain.ChaveRps;
namespace Nfe.Paulistana.Tests.V1.Services;

/// <summary>
/// Testes unitários para <see cref="ConsultaNFeService"/>:
/// guard clauses do construtor e de <see cref="IConsultaNFeService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class ConsultaNFeServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{

    private PedidoConsultaNFe CriarConsultaAssinada()
    {
        var factory = new PedidoConsultaNFeFactory(fixture.Certificado);
        var detalhe = new DetalheConsultaNFe(
            new DomainChaveRps(new InscricaoMunicipal(39616924), new SerieRps("BB"), new Numero(4105)));
        return factory.NewCpf((Cpf)TestConstants.ValidCpf, [detalhe]);
    }

    // Sem payload
    private const string SoapEnvelopeVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaNFeResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaNFeResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoConsulta xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="1"><Sucesso>true</Sucesso></Cabecalho></RetornoConsulta></RetornoXML></ConsultaNFeResponse></soap:Body></soap:Envelope>""";

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_HttpClientNulo_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new ConsultaNFeService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaNFeService(httpClient);
        PedidoConsultaNFe? pedido = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedido!));
    }

    [Fact]
    public async Task SendAsync_PedidoNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaNFeService(httpClient);
        var pedido = new PedidoConsultaNFe();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    // ============================================
    // Resposta do webservice
    // ============================================

    [Fact]
    public async Task SendAsync_RespostaSemPayload_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaNFeService(httpClient);
        PedidoConsultaNFe pedido = CriarConsultaAssinada();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    [Fact]
    public async Task SendAsync_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeService(httpClient);
        PedidoConsultaNFe pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaNFeService(httpClient);
        PedidoConsultaNFe pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }
}