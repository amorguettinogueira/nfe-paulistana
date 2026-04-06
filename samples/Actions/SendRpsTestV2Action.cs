using Microsoft.Extensions.Logging;
using Nfe.Paulistana.Integration.Sample.Configuration;
using Nfe.Paulistana.Integration.Sample.Presentation.Console;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.Integration.Sample.Actions;

/// <summary>
/// Constrói e envia um lote de RPS em <b>modo de teste</b> (<c>modoTeste: true</c>) via
/// serviço SOAP <c>EnvioLoteRPS</c> V02
/// (<see cref="Nfe.Paulistana.V2.Services.IEnvioLoteRpsService"/>).
/// O servidor da Prefeitura processa o RPS sem emitir NF-e definitiva.
/// Além dos campos básicos, suporta os campos adicionais da versão V02:
/// IBS/CBS (<c>CodigoOperacao</c>, <c>ClassificacaoTributaria</c>),
/// <c>ExigibilidadeSuspensa</c>, <c>PagamentoParceladoAntecipado</c>,
/// <c>CodigoNBS</c> e <c>CodigoMunicipioIbge</c>.
/// <para><b>Configuração necessária (<see cref="Configuration.AppSettings"/>):</b>
/// <c>MeuCnpj</c>, <c>MinhaInscricaoMunicipal</c> e todos os campos de
/// <c>EnvioRpsTesteV2</c>, inclusive <c>ExigibilidadeSuspensa</c>,
/// <c>PagamentoParceladoAntecipado</c>, <c>CodigoNBS</c>, <c>CodigoMunicipioIbge</c>,
/// <c>CodigoOperacao</c> e <c>ClassificacaoTributaria</c>.</para>
/// <para><b>Pré-validações:</b> lança <see cref="InvalidOperationException"/> se
/// <c>TributacaoNfe</c> estiver em branco ou se <c>TipoRps</c> não for um valor válido
/// do enum <see cref="Nfe.Paulistana.Models.Enums.TipoRps"/>.</para>
/// <para><b>Efeitos:</b> chamada de rede SOAP com mTLS ao endpoint da Prefeitura de São Paulo.
/// <b>Não idempotente</b> — cada envio gera um protocolo de processamento no servidor,
/// mesmo em modo de teste.</para>
/// </summary>
internal sealed class SendRpsTestV2Action(PedidoEnvioLoteFactory factory,
                                          IEnvioLoteRpsService service,
                                          AppSettings settings,
                                          ILogger<SendRpsTestV2Action> logger) : IIntegrationAction
{
    public int MenuOrder => ActionCatalog.SendRpsTestV2.Order;
    public string Description => ActionCatalog.SendRpsTestV2.Description;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var s = settings.EnvioRpsTesteV2;

            if (string.IsNullOrWhiteSpace(s.TributacaoNfe))
            {
                throw new InvalidOperationException("SendRpsTesteV2: TributacaoNfe não configurado.");
            }

            if (!Enum.TryParse<TipoRps>(s.TipoRps, out var tipoRps))
            {
                throw new InvalidOperationException("SendRpsTesteV2: TipoRps inválido nas configurações.");
            }

            ConsolePresenter.Info("Testando criar RPS (V02) com os seguintes dados:");
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
            ConsolePresenter.Info($"  Exigibilidade Suspensa: {s.ExigibilidadeSuspensa}");
            ConsolePresenter.Info($"  Pagamento Parcelado Antecipado: {s.PagamentoParceladoAntecipado}");
            ConsolePresenter.Info($"  Código NBS: {s.CodigoNBS}");
            ConsolePresenter.Info($"  Código Municipio Ibge: {s.CodigoMunicipioIbge}");
            ConsolePresenter.Info($"  Código Operação: {s.CodigoOperacao}");
            ConsolePresenter.Info($"  Classificação Tributária: {s.ClassificacaoTributaria}");

            var tomador = TomadorBuilder
                .NewCnpj((Cnpj)s.CnpjTomador, (RazaoSocial)s.RazaoSocialTomador)
                .Build();

            var ibsCbs = InformacoesIbsCbsBuilder.New()
                .SetUsoOuConsumoPessoal((NaoSim)false)
                .SetCodigoOperacaoFornecimento((CodigoOperacaoFornecimento)s.CodigoOperacao)
                .SetClassificacaoTributaria((ClassificacaoTributaria)s.ClassificacaoTributaria)
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
                    exigibilidadeSuspensa: (NaoSim)s.ExigibilidadeSuspensa,
                    pagamentoParceladoAntecipado: (NaoSim)s.PagamentoParceladoAntecipado,
                    statusNFe: StatusNfe.Normal)
                .SetServico(
                    codigoServico: (CodigoServico)s.CodigoServico, nbs: (CodigoNBS)s.CodigoNBS)
                .SetIss(
                    aliquota: (Aliquota)s.Aliquota,
                    issRetido: s.IssRetido)
                .SetIbsCbs(ibsCbs)
                .SetValorFinalCobrado((Valor)s.ValorServicos)
                .SetLocalPrestacao((CodigoIbge)s.CodigoMunicipioIbge)
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
            ConsolePrinter.PrintException(ex, logger, "Erro em SendRpsTesteV2Action");
        }
    }
}