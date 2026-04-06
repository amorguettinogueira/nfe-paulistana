using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Cancela uma NF-e de serviços via serviço SOAP <c>CancelamentoNFe</c> V01
/// (<see cref="Nfe.Paulistana.V1.Services.ICancelamentoNFeService"/>).
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MinhaInscricaoMunicipal</c>, <c>MeuCnpj</c>,
/// <c>CancelamentoNfeV1:NumeroNFeParaCancelar</c>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS ao endpoint da Prefeitura de São Paulo.
/// <b>Não idempotente</b> — o cancelamento é registrado permanentemente no servidor;
/// repetir a operação resulta em erro de negócio (NF-e já cancelada).</para>
/// </summary>
internal sealed class CancelNfeV1Action(PedidoCancelamentoNFeFactory factory,
                                        ICancelamentoNFeService service,
                                        AppSettings settings,
                                        ILogger<CancelNfeV1Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.CancelNfeV1.Order;
    public string Description => ActionCatalog.CancelNfeV1.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Minha Inscrição Municipal: {settings.MinhaInscricaoMunicipal}");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  Número da NF-e para Cancelar: {settings.CancelamentoNfeV1.NumeroNFeParaCancelar}");

            DetalheCancelamentoNFe[] detalhes = [
                new(new ChaveNfe(
                    inscricaoPrestador: (InscricaoMunicipal)settings.MinhaInscricaoMunicipal,
                    numeroNFe: (Numero)settings.CancelamentoNfeV1.NumeroNFeParaCancelar))
            ];

            var pedidoCancelamento = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                transacao: false,
                detalhes: detalhes);

            var retorno = await service.SendAsync(pedidoCancelamento, cancellationToken: cancellationToken);

            ConsolePresenter.Outcome($"\nSucesso: {retorno.Cabecalho?.Sucesso} - NF-e cancelada: {settings.CancelamentoNfeV1.NumeroNFeParaCancelar}", retorno.Cabecalho?.Sucesso ?? false);

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
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro ao cancelar NF-e");
        }
    }
}