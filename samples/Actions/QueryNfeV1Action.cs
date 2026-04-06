using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Exceptions;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Consulta os dados de uma NF-e específica por número via serviço SOAP
/// <c>ConsultaNFe</c> V01 (<see cref="Nfe.Paulistana.V1.Services.IConsultaNFeService"/>).
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MeuCnpj</c>, <c>MinhaInscricaoMunicipal</c>,
/// <c>ConsultaNfeV1:NumeroNotaFiscal</c>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS.
/// Idempotente — somente leitura no servidor da Prefeitura.</para>
/// </summary>
internal sealed class QueryNfeV1Action(PedidoConsultaNFeFactory factory,
                                       IConsultaNFeService service,
                                       AppSettings settings,
                                       ILogger<QueryNfeV1Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.QueryNfeV1.Order;
    public string Description => ActionCatalog.QueryNfeV1.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  Minha Inscrição Municipal: {settings.MinhaInscricaoMunicipal}");
            ConsolePresenter.Info($"  Número da Nota Fiscal: {settings.ConsultaNfeV1.NumeroNotaFiscal}");

            DetalheConsultaNFe[] detalhes = [
                new(new ChaveNfe(
                    inscricaoPrestador: (InscricaoMunicipal)settings.MinhaInscricaoMunicipal,
                    numeroNFe: (Numero)settings.ConsultaNfeV1.NumeroNotaFiscal)),
            ];

            var pedidoConsulta = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                detalhes: detalhes);

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
        catch (NfeRequestException ex)
        {
            ConsolePrinter.PrintException(ex, logger, "NFe request falhou");
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro inesperado em ConsultaNFeV1Action");
        }
    }
}