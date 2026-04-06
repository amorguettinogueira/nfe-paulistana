using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Consulta NF-e recebidas em um intervalo de datas via serviço SOAP
/// <c>ConsultaNFeRecebidas</c> V02 (<see cref="Nfe.Paulistana.V2.Services.IConsultaNFeRecebidasService"/>).
/// O período é calculado dinamicamente: de
/// <c>DateTime.Today.AddDays(-ConsultaNDiasAtras)</c> até <c>DateTime.Today</c>.
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MeuCnpj</c>, <c>CnpjDaMinhaFilial</c>, <c>MinhaInscricaoMunicipal</c>,
/// <c>ConsultaNfeRecebidasV2:ConsultaNDiasAtras</c>,
/// <c>ConsultaNfeRecebidasV2:NumeroPagina</c>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS.
/// Idempotente — somente leitura no servidor da Prefeitura.</para>
/// </summary>
internal sealed class QueryReceivedNfeV2Action(PedidoConsultaNFePeriodoFactory factory,
                                               IConsultaNFeRecebidasService service,
                                               AppSettings settings,
                                               ILogger<QueryReceivedNfeV2Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.QueryReceivedNfeV2.Order;
    public string Description => ActionCatalog.QueryReceivedNfeV2.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var dtInicio = DateTime.Today.AddDays(-settings.ConsultaNfeRecebidasV2.ConsultaNDiasAtras);
            var dtFim = DateTime.Today;

            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  CNPJ da Minha Filial: {settings.CnpjDaMinhaFilial}");
            ConsolePresenter.Info($"  Minha Inscrição Municipal: {settings.MinhaInscricaoMunicipal}");
            ConsolePresenter.Info($"  Período: {dtInicio:dd/MM/yyyy} → {dtFim:dd/MM/yyyy}");
            ConsolePresenter.Info($"  Página: {settings.ConsultaNfeRecebidasV2.NumeroPagina}\n");

            var pedidoConsulta = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                cpfCnpj: (CpfOrCnpj)(Cnpj)settings.CnpjDaMinhaFilial,
                inscricao: (InscricaoMunicipal)settings.MinhaInscricaoMunicipal,
                dtInicio: (DataXsd)dtInicio,
                dtFim: (DataXsd)dtFim,
                numeroPagina: (Numero)settings.ConsultaNfeRecebidasV2.NumeroPagina);

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
            ConsolePrinter.PrintException(ex, logger, "Erro em ConsultaNFeRecebidasV2Action");
        }
    }
}