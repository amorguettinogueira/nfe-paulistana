using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Infrastructure;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Faz o download do WSDL do Web Service da NF-e Paulistana via
/// <see cref="Infrastructure.HttpWsdlFetcher"/> e persiste o conteúdo em disco.
/// <para><b>Configuração necessária:</b> nenhuma configuração específica de ação —
/// o certificado em <c>Certificado:CaminhoArquivo</c> / <c>Certificado:Senha</c>
/// é utilizado pelo <c>HttpClient</c> para autenticação mTLS.</para>
/// <para><b>Efeitos:</b> chamada de rede HTTP com mTLS; grava arquivo
/// <c>lotenfe_{yyyyMMddHHmmss}.wsdl</c> (UTF-8) em <see cref="Path.GetTempPath"/>.
/// Idempotente — cada execução cria seu próprio arquivo com timestamp único.</para>
/// </summary>
internal sealed class DownloadWsdlAction(HttpWsdlFetcher wsdlClient,
                                         ILogger<DownloadWsdlAction> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.DownloadWsdl.Order;
    public string Description => ActionCatalog.DownloadWsdl.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var body = await wsdlClient.GetWsdlAsync(cancellationToken);
            var path = Path.Combine(Path.GetTempPath(), $"lotenfe_{DateTime.Now:yyyyMMddHHmmss}.wsdl");
            await File.WriteAllTextAsync(path, body, System.Text.Encoding.UTF8, cancellationToken);
            ConsolePresenter.Success($"\nWSDL completo salvo em: {path}");
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro ao buscar WSDL");
        }
    }
}