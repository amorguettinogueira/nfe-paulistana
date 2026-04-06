using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nfe.Paulistana.Options;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Services;
using System.Security.Cryptography.X509Certificates;
using V2Builders = Nfe.Paulistana.V2.Builders;

namespace Nfe.Paulistana.Extensions;

/// <summary>
/// Extensões de <see cref="IServiceCollection"/> para registrar os serviços da NF-e Paulistana.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string CertificadoNaoConfigurado =
        "O certificado digital não foi configurado. Configure ao menos uma fonte em NfePaulistanaOptions.Certificado.";

    /// <summary>
    /// Registra apenas os serviços V1 da NF-e Paulistana no contêiner de injeção de dependências.
    /// </summary>
    /// <param name="services">O contêiner de serviços.</param>
    /// <param name="configure">
    /// Delegate de configuração das opções. Deve definir ao menos uma fonte de certificado em
    /// <see cref="NfeOptions.Certificado"/>; caso contrário, uma <see cref="InvalidOperationException"/>
    /// será lançada durante o registro.
    /// </param>
    /// <param name="configureClient">
    /// Delegate opcional aplicado a cada <see cref="IHttpClientBuilder"/> registrado, permitindo
    /// configurar políticas de resiliência uniformemente, por exemplo:
    /// <c>b => b.AddStandardResilienceHandler()</c>.
    /// </param>
    /// <returns>O próprio <see cref="IServiceCollection"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="services"/> ou <paramref name="configure"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando nenhuma fonte de certificado é configurada.</exception>
    public static IServiceCollection AddNfePaulistanaV1(
        this IServiceCollection services,
        Action<NfeOptions> configure,
        Action<IHttpClientBuilder>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        (NfeOptions? options, Func<HttpMessageHandler>? handlerFactory) = ValidateAndBuild(configure);
        RegisterV1Clients(services, options, handlerFactory, configureClient);
        return services;
    }

    /// <summary>
    /// Registra apenas os serviços V2 da NF-e Paulistana no contêiner de injeção de dependências.
    /// </summary>
    /// <param name="services">O contêiner de serviços.</param>
    /// <param name="configure">
    /// Delegate de configuração das opções. Deve definir ao menos uma fonte de certificado em
    /// <see cref="NfeOptions.Certificado"/>; caso contrário, uma <see cref="InvalidOperationException"/>
    /// será lançada durante o registro.
    /// </param>
    /// <param name="configureClient">
    /// Delegate opcional aplicado a cada <see cref="IHttpClientBuilder"/> registrado, permitindo
    /// configurar políticas de resiliência uniformemente, por exemplo:
    /// <c>b => b.AddStandardResilienceHandler()</c>.
    /// </param>
    /// <returns>O próprio <see cref="IServiceCollection"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="services"/> ou <paramref name="configure"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando nenhuma fonte de certificado é configurada.</exception>
    public static IServiceCollection AddNfePaulistanaV2(
        this IServiceCollection services,
        Action<NfeOptions> configure,
        Action<IHttpClientBuilder>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        (NfeOptions? options, Func<HttpMessageHandler>? handlerFactory) = ValidateAndBuild(configure);
        RegisterV2Clients(services, options, handlerFactory, configureClient);
        return services;
    }

    /// <summary>
    /// Registra os serviços V1 e V2 da NF-e Paulistana no contêiner de injeção de dependências.
    /// Indicado para cenários de migração gradual onde ambas as versões precisam coexistir.
    /// </summary>
    /// <param name="services">O contêiner de serviços.</param>
    /// <param name="configure">
    /// Delegate de configuração das opções. Deve definir ao menos uma fonte de certificado em
    /// <see cref="NfeOptions.Certificado"/>; caso contrário, uma <see cref="InvalidOperationException"/>
    /// será lançada durante o registro.
    /// </param>
    /// <param name="configureClient">
    /// Delegate opcional aplicado a cada <see cref="IHttpClientBuilder"/> registrado, permitindo
    /// configurar políticas de resiliência uniformemente, por exemplo:
    /// <c>b => b.AddStandardResilienceHandler()</c>.
    /// </param>
    /// <returns>O próprio <see cref="IServiceCollection"/> para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="services"/> ou <paramref name="configure"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando nenhuma fonte de certificado é configurada.</exception>
    public static IServiceCollection AddNfePaulistanaAll(
        this IServiceCollection services,
        Action<NfeOptions> configure,
        Action<IHttpClientBuilder>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        (NfeOptions? options, Func<HttpMessageHandler>? handlerFactory) = ValidateAndBuild(configure);
        RegisterV1Clients(services, options, handlerFactory, configureClient);
        RegisterV2Clients(services, options, handlerFactory, configureClient);
        return services;
    }

    private static (NfeOptions Options, Func<HttpMessageHandler> HandlerFactory) ValidateAndBuild(
        Action<NfeOptions> configure)
    {
        var options = new NfeOptions();
        configure(options);

        Certificado c = options.Certificado;

        return c.FilePath is null && c.PointerHandle is null && c.RawData is null && c.Certificate is null
            ? throw new InvalidOperationException(CertificadoNaoConfigurado)
            : ((NfeOptions Options, Func<HttpMessageHandler> HandlerFactory))(options, HandlerFactory);

        HttpMessageHandler HandlerFactory()
        {
            X509Certificate2 cert = options.Certificado.Build();
            var handler = new HttpClientHandler();
            _ = handler.ClientCertificates.Add(cert);
            return handler;
        }
    }

    private static void RegisterV1Clients(
        IServiceCollection services,
        NfeOptions options,
        Func<HttpMessageHandler> handlerFactory,
        Action<IHttpClientBuilder>? configureClient)
    {
        void Add<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            IHttpClientBuilder builder = services
                .AddHttpClient<TService, TImpl>(c => c.BaseAddress = options.EndpointUrl)
                .ConfigurePrimaryHttpMessageHandler(handlerFactory);
            configureClient?.Invoke(builder);
        }

        Add<IConsultaCNPJService, ConsultaCNPJService>();
        Add<IConsultaInformacoesLoteService, ConsultaInformacoesLoteService>();
        Add<IConsultaLoteService, ConsultaLoteService>();
        Add<IConsultaNFeRecebidasService, ConsultaNFeRecebidasService>();
        Add<IConsultaNFeEmitidasService, ConsultaNFeEmitidasService>();
        Add<IConsultaNFeService, ConsultaNFeService>();
        Add<ICancelamentoNFeService, CancelamentoNFeService>();
        Add<IEnvioRpsService, EnvioRpsService>();
        Add<IEnvioLoteRpsService, EnvioLoteRpsService>();

        services.TryAddSingleton(options.Certificado);
        services.TryAddSingleton<PedidoConsultaCNPJFactory>();
        services.TryAddSingleton<PedidoInformacoesLoteFactory>();
        services.TryAddSingleton<PedidoConsultaLoteFactory>();
        services.TryAddSingleton<PedidoConsultaNFePeriodoFactory>();
        services.TryAddSingleton<PedidoConsultaNFeFactory>();
        services.TryAddSingleton<PedidoCancelamentoNFeFactory>();
        services.TryAddSingleton<PedidoEnvioFactory>();
        services.TryAddSingleton<PedidoEnvioLoteFactory>();
    }

    private static void RegisterV2Clients(
        IServiceCollection services,
        NfeOptions options,
        Func<HttpMessageHandler> handlerFactory,
        Action<IHttpClientBuilder>? configureClient)
    {
        void Add<TService, TImpl>(string name)
            where TService : class
            where TImpl : class, TService
        {
            IHttpClientBuilder builder = services
                .AddHttpClient<TService, TImpl>(name, c => c.BaseAddress = options.EndpointUrl)
                .ConfigurePrimaryHttpMessageHandler(handlerFactory);
            configureClient?.Invoke(builder);
        }

        Add<V2.Services.IConsultaCNPJService, V2.Services.ConsultaCNPJService>("ConsultaCNPJServiceV2");
        Add<V2.Services.IConsultaInformacoesLoteService, V2.Services.ConsultaInformacoesLoteService>("ConsultaInformacoesLoteServiceV2");
        Add<V2.Services.IConsultaLoteService, V2.Services.ConsultaLoteService>("ConsultaLoteServiceV2");
        Add<V2.Services.IConsultaNFeRecebidasService, V2.Services.ConsultaNFeRecebidasService>("ConsultaNFeRecebidasServiceV2");
        Add<V2.Services.IConsultaNFeEmitidasService, V2.Services.ConsultaNFeEmitidasService>("ConsultaNFeEmitidasServiceV2");
        Add<V2.Services.IConsultaNFeService, V2.Services.ConsultaNFeService>("ConsultaNFeServiceV2");
        Add<V2.Services.ICancelamentoNFeService, V2.Services.CancelamentoNFeService>("CancelamentoNFeServiceV2");
        Add<V2.Services.IEnvioRpsService, V2.Services.EnvioRpsService>("EnvioRpsServiceV2");
        Add<V2.Services.IEnvioLoteRpsService, V2.Services.EnvioLoteRpsService>("EnvioLoteRpsServiceV2");

        services.TryAddSingleton(options.Certificado);
        services.TryAddSingleton<V2Builders.PedidoConsultaCNPJFactory>();
        services.TryAddSingleton<V2Builders.PedidoInformacoesLoteFactory>();
        services.TryAddSingleton<V2Builders.PedidoConsultaLoteFactory>();
        services.TryAddSingleton<V2Builders.PedidoConsultaNFePeriodoFactory>();
        services.TryAddSingleton<V2Builders.PedidoConsultaNFeFactory>();
        services.TryAddSingleton<V2Builders.PedidoCancelamentoNFeFactory>();
        services.TryAddSingleton<V2Builders.PedidoEnvioFactory>();
        services.TryAddSingleton<V2Builders.PedidoEnvioLoteFactory>();
    }
}