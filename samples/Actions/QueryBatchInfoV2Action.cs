using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Consulta informações de um lote RPS via serviço SOAP <c>ConsultaInformacoesLote</c> V02
/// (<see cref="Nfe.Paulistana.V2.Services.IConsultaInformacoesLoteService"/>).
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MinhaInscricaoMunicipal</c>, <c>MeuCnpj</c>;
/// <c>ConsultaInformacoesLoteV2:NumeroLote</c> é opcional —
/// quando menor ou igual a zero, a consulta é enviada sem filtro de número de lote.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS.
/// Idempotente — somente leitura no servidor da Prefeitura.</para>
/// </summary>
internal sealed class QueryBatchInfoV2Action(PedidoInformacoesLoteFactory factory,
                                             IConsultaInformacoesLoteService service,
                                             AppSettings settings,
                                             ILogger<QueryBatchInfoV2Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.QueryBatchInfoV2.Order;
    public string Description => ActionCatalog.QueryBatchInfoV2.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var numeroLote = settings.ConsultaInformacoesLoteV2.NumeroLote > 0
                ? (Numero?)settings.ConsultaInformacoesLoteV2.NumeroLote.Value
                : null;

            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Minha Inscrição Municipal: {settings.MinhaInscricaoMunicipal}");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  Número do Lote: {numeroLote}");

            var pedidoConsulta = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                inscricaoPrestador: (InscricaoMunicipal)settings.MinhaInscricaoMunicipal,
                numeroLote: numeroLote);

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

            var info = retorno.Cabecalho?.InformacoesLote;
            if (info is not null)
            {
                ConsolePresenter.Info("Informações do Lote:");
                ConsolePresenter.Info($"  Número Lote:          {info.NumeroLote}");
                ConsolePresenter.Info($"  Inscrição Prestador:  {info.InscricaoPrestador}");
                ConsolePresenter.Info($"  CPF/CNPJ Remetente:   {info.CpfCnpjRemetente}");
                ConsolePresenter.Info($"  Data Envio:           {info.DataEnvioLote:dd/MM/yyyy HH:mm:ss}");
                ConsolePresenter.Info($"  Notas Processadas:    {info.QtdNotasProcessadas}");
                ConsolePresenter.Info($"  Tempo Processamento:  {info.TempoProcessamento} ms");
                ConsolePresenter.Info($"  Valor Total Serviços: {info.ValorTotalServicos}");
                ConsolePresenter.Info($"  Valor Total Deduções: {info.ValorTotalDeducoes}");
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro em ConsultaInformacoesLoteV2Action");
        }
    }
}