using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V2.Services;

/// <summary>
/// Testes unitários para <see cref="EnvioRpsService"/> v02:
/// guard clauses do construtor e de <see cref="IEnvioRpsService.SendAsync"/>,
/// falha na validação XSD e deserialização da resposta do webservice.
/// </summary>
public class EnvioRpsServiceTests
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
            (CodigoOperacaoFornecimento)"123456");

    private static Rps CriarRps() =>
        RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(4105),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', (NaoSim)false, (NaoSim)false, StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), new CodigoNBS("123456789"))
            .SetIss((Aliquota)0.05m, false)
            .SetIbsCbs(CriarInformacoesIbsCbs())
            .SetValorInicialCobrado(new Valor(1000m))
            .SetLocalPrestacao((CodigoIbge)3550308)
            .SetTomador(TomadorPadrao)
            .Build();

    private static PedidoEnvio CriarPedidoAssinado()
    {
        var factory = new PedidoEnvioFactory(CriarConfiguracao());
        return factory.NewCpf(new Cpf(new ValidCpfNumber().Min()), CriarRps());
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
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe" /></soap:Body></soap:Envelope>""";

    // Com payload (Versao=2)
    private const string SoapEnvelopeComRetorno =
        """<?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"><soap:Body><EnvioRPSResponse xmlns="http://www.prefeitura.sp.gov.br/nfe"><RetornoXML><RetornoEnvioRPS xmlns="http://www.prefeitura.sp.gov.br/nfe"><Cabecalho xmlns="" Versao="2"><Sucesso>true</Sucesso></Cabecalho></RetornoEnvioRPS></RetornoXML></EnvioRPSResponse></soap:Body></soap:Envelope>""";

    private sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(response);
    }

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
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio? pedidoEnvio = null;

        _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.SendAsync(pedidoEnvio!));
    }

    [Fact]
    public async Task SendAsync_PedidoEnvioNaoAssinado_ThrowsInvalidOperationException()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
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
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeVazio);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio pedidoEnvio = CriarPedidoAssinado();

        _ = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync(pedidoEnvio));
    }

    [Fact]
    public async Task SendAsync_WebserviceRetornaRespostaValida_RetornaRetornoNaoNulo()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio pedidoEnvio = CriarPedidoAssinado();

        RetornoEnvioRps resultado = await service.SendAsync(pedidoEnvio);

        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task SendAsync_WebserviceRetornaRespostaValida_CabecalhoIndicaSucesso()
    {
        using HttpClient httpClient = CriarHttpClientFake(SoapEnvelopeComRetorno);
        var service = new EnvioRpsService(httpClient);
        PedidoEnvio pedidoEnvio = CriarPedidoAssinado();

        RetornoEnvioRps resultado = await service.SendAsync(pedidoEnvio);

        Assert.True(resultado.Cabecalho?.Sucesso);
    }
}