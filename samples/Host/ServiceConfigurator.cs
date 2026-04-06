using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Integration.Sample.Actions;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Infrastructure;
using Nfe.Paulistana.Options;

namespace Nfe.Paulistana.Integration.Sample.Host;

/// <summary>
/// Ponto único de configuração do container de DI da aplicação.
/// Centraliza o registro de serviços SOAP NF-e Paulistana, clientes HTTP,
/// logging e implementações de <see cref="IIntegrationAction"/>.
/// </summary>
internal static class ServiceConfigurator
{
    /// <summary>
    /// Registra todos os serviços no <paramref name="services"/>.
    /// Deve ser invocado após <see cref="AppSettingsLoader.LoadAndValidate"/>;
    /// o arquivo <c>.pfx</c> precisa estar acessível em disco no momento em que
    /// o <c>HttpClientHandler</c> for resolvido pelo container.
    /// <para><b>Registros e tempos de vida:</b>
    /// <list type="bullet">
    ///   <item><b>Singleton</b> — <see cref="AppSettings"/> (instância pré-validada passada diretamente).</item>
    ///   <item>Console logger via <c>AddLogging/AddConsole</c>.</item>
    ///   <item><b>Singleton</b> — todos os serviços SOAP V01 e V02 registrados por <c>AddNfePaulistanaAll</c>,
    ///         com mTLS via certificado A1 e pipeline de resiliência padrão.</item>
    ///   <item><b>Transient</b> — <see cref="HttpWsdlFetcher"/> via <c>AddHttpClient</c>,
    ///         com <c>HttpClientHandler</c> mTLS construído a partir de <c>CertificadoNfePaulistana</c>
    ///         e pipeline de resiliência padrão.</item>
    ///   <item><b>Singleton</b> — todas as implementações concretas de <see cref="IIntegrationAction"/>
    ///         descobertas por reflexão no assembly de entrada.</item>
    /// </list></para>
    /// <para><b>Pipeline de resiliência (<c>AddStandardResilienceHandler</c>):</b>
    /// retry com backoff exponencial (3 tentativas), circuit-breaker, timeout total de 30 s
    /// e timeout por tentativa de 10 s. Substituível via
    /// <c>Configure&lt;HttpStandardResilienceOptions&gt;</c> no container.</para>
    /// <para><b>Interceptores HTTP:</b> passe delegates adicionais ao parâmetro
    /// <c>configureClient</c> de <c>AddNfePaulistanaAll</c>, ou encadeie
    /// <c>IHttpClientBuilder</c> após <c>AddHttpClient&lt;HttpWsdlFetcher&gt;()</c>.</para>
    /// <para><b>Segurança:</b> <c>settings.Certificado.Senha</c> é repassada apenas a
    /// <c>CertificadoNfePaulistana</c> e nunca gravada em log
    /// (<c>[JsonIgnore]</c> em <see cref="CertificateOptions.Senha"/>).</para>
    /// </summary>
    /// <param name="services">Container de DI a ser configurado.</param>
    /// <param name="settings">Configurações já validadas da aplicação.</param>
    public static void RegisterServices(IServiceCollection services, AppSettings settings)
    {
        services.AddSingleton(settings);

        services.AddLogging(builder =>
        {
            builder.AddConsole();

            // Aplica filtro de nível de log para o categoria SoapDiagnostics lida da variável de ambiente.
            // Defina Logging__LogLevel__SoapDiagnostics=Debug para ver os XMLs trocados com a Prefeitura.
            // No Visual Studio, selecione o perfil "Diagnósticos SOAP" na barra de execução.
            if (Environment.GetEnvironmentVariable("Logging__LogLevel__SoapDiagnostics") is { } levelStr
                && Enum.TryParse<LogLevel>(levelStr, ignoreCase: true, out LogLevel logLevel))
            {
                builder.AddFilter("SoapDiagnostics", logLevel);
            }
        });

        services.AddNfePaulistanaAll(options => options.Certificado = new Certificado
        {
            FilePath = settings.Certificado.CaminhoArquivo,
            Password = settings.Certificado.Senha
        },
        configureClient: b => b
            .AddNfePaulistanaDiagnostics(sp =>
            {
                // Demonstração: captura o XML de cada troca SOAP e registra em nível Debug.
                // Para ver a saída, configure o nível de log para Debug no terminal ou nas variáveis de ambiente:
                //   Logging__LogLevel__SoapDiagnostics=Debug
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("SoapDiagnostics");
                return exchange => logger.LogDebug(
                    "[{SoapAction}] {ElapsedMs}ms | Sucesso: {IsSuccess}\n--- REQUEST ---\n{RequestXml}\n--- RESPONSE ---\n{ResponseXml}",
                    exchange.SoapAction,
                    (long)exchange.Elapsed.TotalMilliseconds,
                    exchange.IsSuccess,
                    exchange.RequestXml,
                    exchange.ResponseXml);
            })
            .AddStandardResilienceHandler());

        services.AddHttpClient<HttpWsdlFetcher>()
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var certificado = sp.GetRequiredService<Certificado>();
                var cert = certificado.Build();

                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(cert);

                return handler;
            })
            .AddStandardResilienceHandler();

        // register all IMenuAction implementations automatically
        foreach (var t in typeof(Program).Assembly.GetTypes().Where(t =>
            typeof(IIntegrationAction).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract))
        {
            services.AddSingleton(typeof(IIntegrationAction), t);
        }
    }
}