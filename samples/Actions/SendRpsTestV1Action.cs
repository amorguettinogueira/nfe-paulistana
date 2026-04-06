using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Constrói e envia um lote de RPS em <b>modo de teste</b> (<c>modoTeste: true</c>) via
/// serviço SOAP <c>EnvioLoteRPS</c> V01
/// (<see cref="Nfe.Paulistana.V1.Services.IEnvioLoteRpsService"/>).
/// O servidor da Prefeitura processa o RPS sem emitir NF-e definitiva.
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MeuCnpj</c>, <c>MinhaInscricaoMunicipal</c> e todos os campos de
/// <c>EnvioRpsTesteV1</c>: <c>CnpjTomador</c>, <c>RazaoSocialTomador</c>, <c>TipoRps</c>,
/// <c>NumeroRps</c>, <c>SerieRps</c>, <c>Discriminacao</c>, <c>CodigoServico</c>,
/// <c>ValorServicos</c>, <c>Aliquota</c>, <c>IssRetido</c>, <c>TributacaoNfe</c>.</para>
/// <para><b>Pré-validações:</b> lança <see cref="InvalidOperationException"/> se
/// <c>TributacaoNfe</c> estiver em branco ou se <c>TipoRps</c> não for um valor válido
/// do enum <see cref="Nfe.Paulistana.Models.Enums.TipoRps"/>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS ao endpoint da Prefeitura de São Paulo.
/// <b>Não idempotente</b> — cada envio gera um protocolo de processamento no servidor,
/// mesmo em modo de teste.</para>
/// </summary>
internal sealed class SendRpsTestV1Action(PedidoEnvioLoteFactory factory,
                                          IEnvioLoteRpsService service,
                                          AppSettings settings,
                                          ILogger<SendRpsTestV1Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.SendRpsTestV1.Order;
    public string Description => ActionCatalog.SendRpsTestV1.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var s = settings.EnvioRpsTesteV1;

            if (string.IsNullOrWhiteSpace(s.TributacaoNfe))
            {
                throw new InvalidOperationException("SendRpsTesteV1: TributacaoNfe não configurado.");
            }

            if (!Enum.TryParse<TipoRps>(s.TipoRps, out var tipoRps))
            {
                throw new InvalidOperationException("SendRpsTesteV1: TipoRps inválido nas configurações.");
            }

            ConsolePresenter.Info("Testando criar RPS com os seguintes dados:");
            ConsolePresenter.Info($"  Meu CNPJ: {settings.MeuCnpj}");
            ConsolePresenter.Info($"  Minha Inscrição Municipal: {settings.MinhaInscricaoMunicipal}");
            ConsolePresenter.Info($"  CNPJ do Tomador: {s.CnpjTomador}");
            ConsolePresenter.Info($"  Razão Social do Tomador: {s.RazaoSocialTomador}");
            ConsolePresenter.Info($"  Número do RPS: {s.NumeroRps}");
            ConsolePresenter.Info($"  Série do RPS: {s.SerieRps}");
            ConsolePresenter.Info($"  Discriminação: {s.Discriminacao}");
            ConsolePresenter.Info($"  Código do Serviço: {s.CodigoServico}");
            ConsolePresenter.Info($"  Valor dos Serviços: {s.ValorServicos}");
            ConsolePresenter.Info($"  Alíquota: {s.Aliquota}");
            ConsolePresenter.Info($"  ISS Retido: {s.IssRetido}");
            ConsolePresenter.Info($"  Tributação NFe: {s.TributacaoNfe}");

            var tomador = TomadorBuilder
                .NewCnpj((Cnpj)s.CnpjTomador, (RazaoSocial)s.RazaoSocialTomador)
                .Build();

            var rps = RpsBuilder
                .New(
                    inscricaoPrestador: (InscricaoMunicipal)settings.MinhaInscricaoMunicipal,
                    tipoRps: tipoRps,
                    numeroRps: (Numero)s.NumeroRps,
                    discriminacao: (Discriminacao)s.Discriminacao,
                    serieRps: (SerieRps)s.SerieRps)
                .SetNFe(
                    dataEmissao: new DataXsd(DateTime.Today),
                    tributacaoNFe: (TributacaoNfe)s.TributacaoNfe[0],
                    statusNFe: StatusNfe.Normal)
                .SetServico(
                    codigoServico: (CodigoServico)s.CodigoServico,
                    valorServicos: (Valor)s.ValorServicos)
                .SetIss(
                    aliquota: (Aliquota)s.Aliquota,
                    issRetido: s.IssRetido)
                .SetTomador(tomador)
                .Build();

            var pedidoEnvioLote = factory.NewCnpj(
                cnpj: (Cnpj)settings.MeuCnpj,
                transacao: false,
                rpsList: [rps]);

            var retorno = await service.SendAsync(pedidoEnvioLote, modoTeste: true, cancellationToken: cancellationToken);

            ConsolePresenter.Outcome($"\nSucesso: {retorno.Cabecalho?.Sucesso}", retorno.Cabecalho?.Sucesso ?? false);
            ConsolePrinter.PrintErrosAlertas(retorno.Erro, retorno.Alerta);

            if (retorno.ChavesNFeRps?.Length > 0)
            {
                ConsolePresenter.Info("NFS-e emitidas:");
                foreach (var chave in retorno.ChavesNFeRps)
                {
                    ConsolePresenter.Info($"  Chave: {chave}");
                }
            }
            else
            {
                ConsolePrinter.PrintNenhumRegistro();
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.PrintException(ex, logger, "Erro em SendRpsTesteV1Action");
        }
    }
}