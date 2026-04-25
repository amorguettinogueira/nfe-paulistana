using Microsoft.Extensions.DependencyInjection;
using Nfe.Paulistana.Constants;
using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Services;
using System.Collections.ObjectModel;
using V2Builders = Nfe.Paulistana.V2.Builders;
using V2Services = Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Tests.Extensions;

/// <summary>
/// Testes unitários para <see cref="ServiceCollectionExtensions"/>:
/// guard clauses, validação de certificado, verificação de registro de serviços
/// e invocação do delegate <c>configureClient</c>.
/// </summary>
public class ServiceCollectionExtensionsTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{

    // ============================================
    // Guard clauses — testados via V1 (lógica compartilhada em ValidateAndBuild)
    // ============================================

    [Fact]
    public void AddNfePaulistanaV1_ServicesNulo_ThrowsArgumentNullException()
    {
        IServiceCollection? services = null;

        _ = Assert.Throws<ArgumentNullException>(() => services!.AddNfePaulistanaV1(_ => { }));
    }

    [Fact]
    public void AddNfePaulistanaV1_ConfigureNulo_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        _ = Assert.Throws<ArgumentNullException>(() => services.AddNfePaulistanaV1(null!));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoNaoConfigurado_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();

        _ = Assert.Throws<InvalidOperationException>(() => services.AddNfePaulistanaV1(_ => { }));
    }

    // ============================================
    // Registro de serviços V1
    // ============================================

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIEnvioRpsService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IEnvioRpsService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIEnvioLoteRpsService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IEnvioLoteRpsService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraICancelamentoNFeService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(ICancelamentoNFeService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIConsultaNFeRecebidasService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IConsultaNFeRecebidasService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIConsultaNFeEmitidasService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IConsultaNFeEmitidasService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIConsultaLoteService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IConsultaLoteService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIConsultaInformacoesLoteService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IConsultaInformacoesLoteService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoFilePath_RegistraIConsultaCNPJService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IConsultaCNPJService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoX509_RegistraIEnvioLoteRpsService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.Certificate = fixture.Certificate);

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IEnvioLoteRpsService>());
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoRawData_RegistraIEnvioLoteRpsService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.RawData = new ReadOnlyCollection<byte>([0x01, 0x02, 0x03]));

        Assert.Contains(services, s => s.ServiceType == typeof(IEnvioLoteRpsService));
    }

    [Fact]
    public void AddNfePaulistanaV1_CertificadoPointerHandle_RegistraIEnvioLoteRpsService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.PointerHandle = new IntPtr(1));

        Assert.Contains(services, s => s.ServiceType == typeof(IEnvioLoteRpsService));
    }

    [Fact]
    public void AddNfePaulistanaV1_NaoRegistraServicosV2()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.DoesNotContain(services, s => s.ServiceType == typeof(V2Services.IConsultaCNPJService));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(V2Services.IConsultaInformacoesLoteService));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(V2Services.IConsultaLoteService));
    }

    // ============================================
    // Registro de serviços V2
    // ============================================

    [Fact]
    public void AddNfePaulistanaV2_CertificadoFilePath_RegistraIConsultaCNPJService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV2(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(V2Services.IConsultaCNPJService));
    }

    [Fact]
    public void AddNfePaulistanaV2_CertificadoFilePath_RegistraIConsultaInformacoesLoteService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV2(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(V2Services.IConsultaInformacoesLoteService));
    }

    [Fact]
    public void AddNfePaulistanaV2_CertificadoFilePath_RegistraIConsultaLoteService()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV2(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(V2Services.IConsultaLoteService));
    }

    [Fact]
    public void AddNfePaulistanaV2_NaoRegistraServicosV1()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV2(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.DoesNotContain(services, s => s.ServiceType == typeof(IEnvioRpsService));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(IEnvioLoteRpsService));
    }

    // ============================================
    // Registro combinado — AddNfePaulistanaAll
    // ============================================

    [Fact]
    public void AddNfePaulistanaAll_RegistraServicosV1eV2()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaAll(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(IEnvioRpsService));
        Assert.Contains(services, s => s.ServiceType == typeof(IConsultaLoteService));
        Assert.Contains(services, s => s.ServiceType == typeof(V2Services.IConsultaCNPJService));
        Assert.Contains(services, s => s.ServiceType == typeof(V2Services.IConsultaInformacoesLoteService));
        Assert.Contains(services, s => s.ServiceType == typeof(V2Services.IConsultaLoteService));
    }

    // ============================================
    // Registro de CertificadoNfePaulistana e Factories
    // ============================================

    [Fact]
    public void AddNfePaulistanaV1_RegistraCertificadoNfePaulistanaSingleton()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(Certificado)
            && s.Lifetime == ServiceLifetime.Singleton);
    }

    [Theory]
    [InlineData(typeof(PedidoEnvioFactory))]
    [InlineData(typeof(PedidoEnvioLoteFactory))]
    [InlineData(typeof(PedidoConsultaCNPJFactory))]
    [InlineData(typeof(PedidoConsultaNFeFactory))]
    [InlineData(typeof(PedidoConsultaNFePeriodoFactory))]
    [InlineData(typeof(PedidoConsultaLoteFactory))]
    [InlineData(typeof(PedidoInformacoesLoteFactory))]
    [InlineData(typeof(PedidoCancelamentoNFeFactory))]
    public void AddNfePaulistanaV1_RegistraFactoryV1ComoSingleton(Type factoryType)
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == factoryType
            && s.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddNfePaulistanaV1_NaoRegistraFactoriesV2()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.DoesNotContain(services, s => s.ServiceType == typeof(V2Builders.PedidoConsultaCNPJFactory));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(V2Builders.PedidoInformacoesLoteFactory));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(V2Builders.PedidoConsultaLoteFactory));
    }

    [Theory]
    [InlineData(typeof(V2Builders.PedidoConsultaCNPJFactory))]
    [InlineData(typeof(V2Builders.PedidoInformacoesLoteFactory))]
    [InlineData(typeof(V2Builders.PedidoConsultaLoteFactory))]
    public void AddNfePaulistanaV2_RegistraFactoryV2ComoSingleton(Type factoryType)
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV2(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == factoryType
            && s.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddNfePaulistanaV2_NaoRegistraFactoriesV1()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV2(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.DoesNotContain(services, s => s.ServiceType == typeof(PedidoEnvioFactory));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(PedidoEnvioLoteFactory));
    }

    [Fact]
    public void AddNfePaulistanaAll_RegistraFactoriesV1eV2()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaAll(opt => opt.Certificado.FilePath = "cert.pfx");

        Assert.Contains(services, s => s.ServiceType == typeof(PedidoEnvioFactory));
        Assert.Contains(services, s => s.ServiceType == typeof(PedidoCancelamentoNFeFactory));
        Assert.Contains(services, s => s.ServiceType == typeof(V2Builders.PedidoConsultaCNPJFactory));
        Assert.Contains(services, s => s.ServiceType == typeof(V2Builders.PedidoInformacoesLoteFactory));
        Assert.Contains(services, s => s.ServiceType == typeof(V2Builders.PedidoConsultaLoteFactory));
    }

    [Fact]
    public void AddNfePaulistanaAll_CertificadoNfePaulistanaRegistradoUmaUnicaVez()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaAll(opt => opt.Certificado.FilePath = "cert.pfx");

        int count = services.Count(s => s.ServiceType == typeof(Certificado));
        Assert.Equal(1, count);
    }

    [Fact]
    public void AddNfePaulistanaV1_FactoryResolvidaPeloContainer()
    {
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt => opt.Certificado.Certificate = fixture.Certificate);

        using ServiceProvider provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<PedidoEnvioFactory>());
    }

    // ============================================
    // Delegate configureClient
    // ============================================

    [Fact]
    public void AddNfePaulistanaV1_ConfigureClient_EInvocadoParaCadaClienteRegistrado()
    {
        var services = new ServiceCollection();
        int invocacoes = 0;

        _ = services.AddNfePaulistanaV1(
            opt => opt.Certificado.FilePath = "cert.pfx",
            _ => invocacoes++);

        Assert.Equal(9, invocacoes);
    }

    [Fact]
    public void AddNfePaulistanaV2_ConfigureClient_EInvocadoParaCadaClienteRegistrado()
    {
        var services = new ServiceCollection();
        int invocacoes = 0;

        _ = services.AddNfePaulistanaV2(
            opt => opt.Certificado.FilePath = "cert.pfx",
            _ => invocacoes++);

        Assert.Equal(9, invocacoes);
    }

    [Fact]
    public void AddNfePaulistanaAll_ConfigureClient_EInvocadoParaTodosOsClientes()
    {
        var services = new ServiceCollection();
        int invocacoes = 0;

        _ = services.AddNfePaulistanaAll(
            opt => opt.Certificado.FilePath = "cert.pfx",
            _ => invocacoes++);

        Assert.Equal(18, invocacoes);
    }

    // ============================================
    // Configuração de endpoint
    // ============================================

    [Fact]
    public void AddNfePaulistanaV1_SemEndpointUrl_UsaUrlDeProducao()
    {
        Uri? endpointCapturado = null;
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt =>
        {
            opt.Certificado.FilePath = "cert.pfx";
            endpointCapturado = opt.EndpointUrl;
        });

        Assert.Equal(new Uri(Endpoints.Producao), endpointCapturado);
    }

    [Fact]
    public void AddNfePaulistanaV1_ComEndpointUrlCustomizado_UsaUrlFornecida()
    {
        var urlCustomizada = new Uri("https://homologacao.prefeitura.sp.gov.br/ws/lotenfe.asmx");
        Uri? endpointCapturado = null;
        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaV1(opt =>
        {
            opt.Certificado.FilePath = "cert.pfx";
            opt.EndpointUrl = urlCustomizada;
            endpointCapturado = opt.EndpointUrl;
        });

        Assert.Equal(urlCustomizada, endpointCapturado);
    }
}