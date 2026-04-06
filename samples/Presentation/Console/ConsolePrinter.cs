using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Exceptions;

namespace Nfe.Paulistana.Integration.Sample.Presentation.Console;

/// <summary>
/// Helper de apresentação para saída de respostas NF-e no console.
/// Combina logging estruturado via <see cref="Microsoft.Extensions.Logging.ILogger"/>
/// com saída colorida via <see cref="ConsolePresenter"/>.
/// </summary>
internal static class ConsolePrinter
{
    /// <summary>
    /// Registra <paramref name="ex"/> no <paramref name="logger"/> (se fornecido) e
    /// exibe detalhes amigáveis no console. Erros de logging são suprimidos para não
    /// ocultar a exceção original. Para <see cref="Nfe.Paulistana.Exceptions.NfeRequestException"/>,
    /// exibe também o payload da requisição e o corpo da resposta SOAP.
    /// </summary>
    public static void PrintException(Exception ex, ILogger? logger, string? message = null)
    {
        try
        {
            if (logger is not null)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    logger.LogError(ex, "{message}", message);
                }
                else
                {
                    logger.LogError(ex, "Erro durante operação");
                }
            }
        }
        catch
        {
            // swallow logging errors to avoid hiding the original exception
        }

        // still print user-friendly details to console
        if (ex is NfeRequestException nfe)
        {
            ConsolePresenter.Error($"Erro: {nfe.Message}");
            ConsolePresenter.Info($"Request Payload: {nfe.RequestPayload}");
            ConsolePresenter.Info($"Response Body: {nfe.ResponseBody}");
        }
        else
        {
            ConsolePresenter.Error($"Erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Exibe erros e alertas retornados por respostas SOAP.
    /// Itera os arrays dinâmicos <paramref name="erros"/> e <paramref name="alertas"/>,
    /// imprimindo código e descrição de cada entrada.
    /// </summary>
    public static void PrintErrosAlertas(dynamic? erros, dynamic? alertas)
    {
        if (erros?.Length > 0)
        {
            ConsolePresenter.Error("Erros:");
            foreach (var erro in erros)
            {
                ConsolePresenter.Error($"  [{erro.Codigo}] {erro.Descricao}");
            }
        }

        if (alertas?.Length > 0)
        {
            ConsolePresenter.Warning("Alertas:");
            foreach (var alerta in alertas)
            {
                ConsolePresenter.Warning($"  [{alerta.Codigo}] {alerta.Descricao}");
            }
        }
    }

    /// <summary>Exibe mensagem padrão de ausência de registros na resposta.</summary>
    public static void PrintNenhumRegistro() =>
        ConsolePresenter.Info("Nenhum registro retornado.");

    /// <summary>
    /// Exibe lista formatada de NFS-e retornadas por serviços de consulta.
    /// Para cada item exibe: número, prestador, tomador, data de emissão,
    /// serviço, valores (ISS, alíquota) e status. Chama <see cref="PrintNenhumRegistro"/>
    /// se a lista estiver vazia ou nula.
    /// </summary>
    public static void PrintNfes(dynamic? nfes)
    {
        if (nfes?.Length > 0)
        {
            ConsolePresenter.Info($"NFS-e retornadas: {nfes.Length}");
            foreach (var nfe in nfes)
            {
                ConsolePresenter.Info($"  NFS-e #{nfe.ChaveNFe?.NumeroNFe}");
                ConsolePresenter.Info($"    Prestador: {nfe.RazaoSocialPrestador} (IM: {nfe.ChaveNFe?.InscricaoPrestador})");
                ConsolePresenter.Info($"    Tomador:   {nfe.RazaoSocialTomador}");
                ConsolePresenter.Info($"    Emissão:   {nfe.DataEmissaoNFe:dd/MM/yyyy}");
                ConsolePresenter.Info($"    Serviço:   {nfe.CodigoServico} — {nfe.Discriminacao}");
                ConsolePresenter.Info($"    Valor:     {nfe.ValorServicos}  ISS: {nfe.ValorIss}  Alíquota: {nfe.AliquotaServicos}");
                ConsolePresenter.Info($"    Status:    {nfe.StatusNFe}");
            }
        }
        else
        {
            PrintNenhumRegistro();
        }
    }
}