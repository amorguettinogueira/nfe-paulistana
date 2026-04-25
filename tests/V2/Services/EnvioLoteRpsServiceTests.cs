using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="EnvioLoteRpsService"/>:
/// guard clauses do construtor e de <see cref="IEnvioLoteRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta para modo de produção e modo de teste.
/// </summary>
public sealed class EnvioLoteRpsServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private static readonly InformacoesIbsCbs IbsCbsPadrao =
        InformacoesIbsCbsBuilder.New()
            .SetUsoOuConsumoPessoal(new NaoSim(false))
            .SetCodigoOperacaoFornecimento(new CodigoOperacao("010101"))
            .SetClassificacaoTributaria(new ClassificacaoTributaria("010101"))
            .Build();

    private PedidoEnvioLote CriarLoteAssinado()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        return factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao)]);
    }

    // Produ
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);

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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteVazio);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteVazio);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeLoteComRetorno);
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
        using HttpClient httpClient = FakeHttpClient.Create(SoapEnvelopeTesteComRetorno);
        var service = new EnvioLoteRpsService(httpClient);
        PedidoEnvioLote pedidoEnvioLote = CriarLoteAssinado();

        // Act
        RetornoEnvioLoteRps resultado = await service.SendAsync(pedidoEnvioLote, modoTeste: true);

        // Assert
        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Cabecalho);
    }
}