using Microsoft.Extensions.DependencyInjection;
using Nfe.Paulistana.Diagnostics;
using Nfe.Paulistana.Extensions;

namespace Nfe.Paulistana.Tests.Extensions;

/// <summary>
/// Testes unitários para <see cref="HttpClientBuilderExtensions"/>.
/// </summary>
public sealed class HttpClientBuilderExtensionsTests
{
    private static IHttpClientBuilder CriarBuilder() =>
        new ServiceCollection().AddHttpClient("test");

    // ── Sobrecarga Action<SoapExchange> ──────────────────────────────────────

    [Fact]
    public void AddNfePaulistanaDiagnostics_BuilderNulo_LancaArgumentNullException() =>
        Assert.ThrowsAny<ArgumentNullException>(() =>
            Nfe.Paulistana.Extensions.HttpClientBuilderExtensions.AddNfePaulistanaDiagnostics(
                null!,
                (Action<SoapExchange>)(_ => { })));

    [Fact]
    public void AddNfePaulistanaDiagnostics_CallbackNulo_LancaArgumentNullException() =>
        Assert.ThrowsAny<ArgumentNullException>(() =>
            CriarBuilder().AddNfePaulistanaDiagnostics(
                (Action<SoapExchange>)null!));

    [Fact]
    public void AddNfePaulistanaDiagnostics_ArgumentosValidos_RetornaOMesmoBuilder()
    {
        // Arrange
        var builder = CriarBuilder();

        // Act
        var resultado = builder.AddNfePaulistanaDiagnostics(_ => { });

        // Assert
        Assert.Same(builder, resultado);
    }

    // ── Sobrecarga Func<IServiceProvider, Action<SoapExchange>> ─────────────

    [Fact]
    public void AddNfePaulistanaDiagnostics_FabricaComBuilderNulo_LancaArgumentNullException() =>
        Assert.ThrowsAny<ArgumentNullException>(() =>
            Nfe.Paulistana.Extensions.HttpClientBuilderExtensions.AddNfePaulistanaDiagnostics(
                null!,
                (Func<IServiceProvider, Action<SoapExchange>>)(_ => _ => { })));

    [Fact]
    public void AddNfePaulistanaDiagnostics_FabricaNula_LancaArgumentNullException() =>
        Assert.ThrowsAny<ArgumentNullException>(() =>
            CriarBuilder().AddNfePaulistanaDiagnostics(
                (Func<IServiceProvider, Action<SoapExchange>>)null!));

    [Fact]
    public void AddNfePaulistanaDiagnostics_FabricaComArgumentosValidos_RetornaOMesmoBuilder()
    {
        // Arrange
        var builder = CriarBuilder();

        // Act
        var resultado = builder.AddNfePaulistanaDiagnostics(_ => _ => { });

        // Assert
        Assert.Same(builder, resultado);
    }

    // ── Cobertura dos lambdas internos (fabrica de handler) ─────────────────

    [Fact]
    public void AddNfePaulistanaDiagnostics_QuandoClienteCriado_InstanciaHandlerDoCallback()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHttpClient("test").AddNfePaulistanaDiagnostics(_ => { });

        // Act
        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        using var client = factory.CreateClient("test");

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void AddNfePaulistanaDiagnostics_QuandoClienteCriado_InstanciaHandlerDaFabrica()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHttpClient("test").AddNfePaulistanaDiagnostics(_ => _ => { });

        // Act
        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        using var client = factory.CreateClient("test");

        // Assert
        Assert.NotNull(client);
    }
}
