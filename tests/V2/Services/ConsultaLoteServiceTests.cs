using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="ConsultaLoteService"/> v02:
/// guard clauses do construtor e de <see cref="IConsultaLoteService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class ConsultaLoteServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private PedidoConsultaLote CriarConsultaAssinada()
    {
        var factory = new PedidoConsultaLoteFactory(fixture.Certificado);
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var numeroLote = new Numero(12345);
        return factory.NewCpf(cpf, numeroLote);
    }

    private PedidoConsultaLote CriarConsultaAssinadaComCnpj()
    {
        var factory = new PedidoConsultaLoteFactory(fixture.Certificado);
        var cnpj = new Cnpj(new ValidCnpjStrings().FirstOrDefault([string.Empty, string.Empty])?.FirstOrDefault(string.Empty).ToString() ?? string.Empty);
        var numeroLote = new Numero(12345);
        return factory.NewCnpj(cnpj, numeroLote);
    }

    // Sem payload
    private const string SoapEnvelopeVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaLoteResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload (Versao=2)
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><ConsultaLoteResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoConsulta xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho><NFe xmlns=""><ChaveNFe><InscricaoPrestador>39616924</InscricaoPrestador><NumeroNFe>1000</NumeroNFe></ChaveNFe><DataEmissaoNFe>2024-01-15T10:30:00</DataEmissaoNFe><DataFatoGeradorNFe>2024-01-15T00:00:00</DataFatoGeradorNFe><StatusNFe>N</StatusNFe><ISSRetido>false</ISSRetido></NFe></RetornoConsulta></RetornoXML></ConsultaLoteResponse></soap:Body></soap:Envelope>""";

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_HttpClientNulo_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new ConsultaLoteService(null!));
    }

    // ============================================
    // Guard clauses — SendAsync
    // ============================================

    [Fact]
    public async Task SendAsync_PedidoNulo_ThrowsArgumentNullException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote? pedido = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedido!));
    }

    [Fact]
    public async Task SendAsync_PedidoNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeVazio);
        var service = new ConsultaLoteService(httpClient);
        var pedido = new PedidoConsultaLote();

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
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinada();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedido));
    }

    [Fact]
    public async Task SendAsync_RespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_CabecalhoVersaoIgualADois()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.Equal(2, resultado.Cabecalho?.Versao);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_NfesContemUmaEntrada()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado.Nfes);
        Assert.Single(resultado.Nfes);
    }

    [Fact]
    public async Task SendAsync_RespostaValida_NfeContemChaveNFe()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinada();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.Equal("1000", resultado.Nfes?[0].ChaveNFe?.NumeroNFe);
        Assert.Equal("39616924", resultado.Nfes?[0].ChaveNFe?.InscricaoPrestador);
    }

    // ============================================
    // Factory — CNPJ alfanumérico
    // ============================================

    [Fact]
    public async Task SendAsync_CriadoComCnpjAlfanumerico_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeComRetorno);
        var service = new ConsultaLoteService(httpClient);
        PedidoConsultaLote pedido = CriarConsultaAssinadaComCnpj();

        RetornoConsulta resultado = await service.SendAsync(pedido);

        Assert.NotNull(resultado);
    }
}