using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Consulta os dados cadastrais de um contribuinte por CNPJ via serviço SOAP
/// <c>ConsultaCNPJ</c> V02 (<see cref="Nfe.Paulistana.V2.Services.IConsultaCNPJService"/>).
/// Exibe adicionalmente as Inscrições Municipais vinculadas ao CNPJ e a flag
/// <c>EmiteNFe</c> de cada inscrição.
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MeuCnpj</c>, <c>ConsultaCnpjV2:CnpjAConsultar</c>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS.
/// Idempotente — somente leitura no servidor da Prefeitura.</para>
/// </summary>
internal sealed class QueryCnpjV2Action(PedidoConsultaCNPJFactory factory,
                                        IConsultaCNPJService service,
                                        AppSettings settings,
                                        ILogger<QueryCnpjV2Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.QueryCnpjV2.Order;
    public string Description => ActionCatalog.QueryCnpjV2.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ConsolePresenter.Info("Parâmetros de entrada:");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  CNPJ a Consultar: {settings.ConsultaCnpjV2.CnpjAConsultar}");

            var pedidoConsulta = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                cnpjContribuinte: (CpfOrCnpj)(Cnpj)settings.ConsultaCnpjV2.CnpjAConsultar);

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

            if (retorno.Detalhe?.Length > 0)
            {
                ConsolePresenter.Info($"Inscrições Municipais vinculadas: {retorno.Detalhe.Length}");
                foreach (var detalhe in retorno.Detalhe)
                {
                    ConsolePresenter.Info($"  CCM: {detalhe.InscricaoMunicipal}  Emite NFS-e: {detalhe.EmiteNFe}");
                }
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro em ConsultaCnpjV2Action");
        }
    }
}