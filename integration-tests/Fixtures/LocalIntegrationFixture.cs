using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.IntegrationTests.Configuration;
using Nfe.Paulistana.Options;

namespace Nfe.Paulistana.IntegrationTests.Fixtures;

/// <summary>
/// Fixture compartilhada dos testes de integração local: carrega os User Secrets,
/// constrói o container de DI com todos os serviços SOAP V1 e V2 e expõe os dados
/// de configuração aos testes.
/// <para>Quando os segredos não estão configurados ou o certificado não é encontrado,
/// <see cref="IsConfigured"/> é <see langword="false"/> e os testes chamam
/// <c>Skip.IfNot(fixture.IsConfigured, ...)</c> para pular graciosamente.</para>
/// </summary>
public sealed class LocalIntegrationFixture : IDisposable
{
    private readonly ServiceProvider? _serviceProvider;

    /// <summary>
    /// Indica se os User Secrets foram encontrados e o certificado existe em disco.
    /// </summary>
    internal bool IsConfigured { get; }

    /// <summary>
    /// Configurações carregadas dos User Secrets. Nunca nulo — campos serão vazios quando não configurados.
    /// </summary>
    internal IntegrationTestSettings Settings { get; }

    public LocalIntegrationFixture()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<LocalIntegrationFixture>()
            .Build();

        Settings = config.Get<IntegrationTestSettings>() ?? new IntegrationTestSettings();
        IsConfigured = Settings.EstaoConfigurados;

        if (!IsConfigured) { return; }

        var services = new ServiceCollection();
        _ = services.AddNfePaulistanaAll(opts => opts.Certificado = new Certificado
        {
            FilePath = Settings.Certificado.CaminhoArquivo,
            Password = Settings.Certificado.Senha,
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Resolve um serviço do container de DI. Apenas válido quando <see cref="IsConfigured"/> é verdadeiro.
    /// </summary>
    internal T GetService<T>() where T : class
        => _serviceProvider!.GetRequiredService<T>();

    public void Dispose()
        => _serviceProvider?.Dispose();
}