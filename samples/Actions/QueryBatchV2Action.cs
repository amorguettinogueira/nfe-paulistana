using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Consulta um lote RPS processado via serviço SOAP <c>ConsultaLote</c> V02
/// (<see cref="Nfe.Paulistana.V2.Services.IConsultaLoteService"/>).
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MeuCnpj</c>, <c>ConsultaLoteV2:NumeroLote</c>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS; exibe as NF-es do lote
/// via <see cref="Presentation.Console.ConsolePrinter.PrintNfes"/>.
/// Idempotente — somente leitura no servidor da Prefeitura.</para>
/// </summary>
internal sealed class QueryBatchV2Action(PedidoConsultaLoteFactory factory,
                                         IConsultaLoteService service,
                                         AppSettings settings,
                                         ILogger<QueryBatchV2Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.QueryBatchV2.Order;
    public string Description => ActionCatalog.QueryBatchV2.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  Número do Lote: {settings.ConsultaLoteV2.NumeroLote}");

            var pedidoConsulta = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                numeroLote: (Numero)settings.ConsultaLoteV2.NumeroLote);

            var retorno = await service.SendAsync(pedidoConsulta, cancellationToken: cancellationToken);

            ConsolePresenter.Outcome($"\nSucesso: {retorno.Cabecalho?.Sucesso}", retorno.Cabecalho?.Sucesso ?? false);

            if (retorno.Erro?.Length > 0)
            {
                ConsolePresenter.Error("Erros:");
                foreach (var erro in retorno.Erro)
                {
                    ConsolePresenter.Error($"  [{erro.Codigo}] {erro.Descricao}");
                }
            }

            if (retorno.Alerta?.Length > 0)
            {
                ConsolePresenter.Warning("Alertas:");
                foreach (var alerta in retorno.Alerta)
                {
                    ConsolePresenter.Warning($"  [{alerta.Codigo}] {alerta.Descricao}");
                }
            }

            ConsolePrinter.PrintNfes(retorno.Nfes);
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro em ConsultaLoteV2Action");
        }
    }
}