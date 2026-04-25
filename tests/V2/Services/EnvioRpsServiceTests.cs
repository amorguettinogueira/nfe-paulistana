using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="EnvioRpsService"/> v02:
/// guard clauses do construtor e de <see cref="IEnvioRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class EnvioRpsServiceTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private static ClassificacaoTributaria ClassificacaoTributariaPadrao =>
        new ClassificacaoTributaria("123456");

    private static InformacoesIbsCbs CriarInformacoesIbsCbs() =>
        new InformacoesIbsCbs(
            FinalidadeEmissaoNfe.NfseRegular,
            (NaoSim)false,
            DestinatarioServicos.ProprioTomador,
            new Valores(
                new TributosIbsCbs(
                    new TributoIbsCbs(ClassificacaoTributariaPadrao, new TributoRegular(ClassificacaoTributariaPadrao)))),
            (CodigoOperacao)"123456");

    private PedidoEnvio CriarPedidoAssinado()
    {
        var factory = new PedidoEnvioFactory(fixture.Certificado);
        return factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao(ibsCbs: CriarInformacoesIbsCbs()));
    }

    // Sem payload
    private const string SoapEnvelopeVazio =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload (Versao=2)
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioRPS></RetornoXML></EnvioRPSResponse></soap:Body></soap:Envelope>""";

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