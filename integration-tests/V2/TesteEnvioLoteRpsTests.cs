using Nfe.Paulistana.IntegrationTests.Configuration;
using Nfe.Paulistana.IntegrationTests.Fixtures;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.V2.Services;

namespace Nfe.Paulistana.IntegrationTests.V2;

/// <summary>
/// Testes de integração Tier 3 para o pipeline completo de envio de RPS V2:
/// constrói um RPS com os campos exclusivos da Reforma Tributária 2026 (IBS/CBS, NBS, localização),
/// assina com certificado A1 pessoal e envia ao endpoint <c>TesteEnvioLoteRPS</c> da Prefeitura
/// (<c>modoTeste: true</c> — o servidor valida sem emitir NF-e definitiva).
/// Requer User Secrets configurados — pulados automaticamente em sua ausência.
/// </summary>
public sealed class TesteEnvioLoteRpsTests(LocalIntegrationFixture fixture)
    : IClassFixture<LocalIntegrationFixture>
{
    // ============================================
    // TesteEnvioLoteRPS — pipeline completo V2
    // ============================================

    [SkippableFact]
    public async Task TesteEnvioLoteRps_RpsComCamposV2_ServidorRespondeSemExcecao()
    {
        Skip.IfNot(fixture.IsConfigured,
            "User Secrets não configurados — execute 'dotnet user-secrets set' no projeto integration-tests.");

        // Arrange
        IntegrationTestSettings s = fixture.Settings;

        Tomador tomador = TomadorBuilder
            .NewCnpj((Cnpj)s.CnpjTomador, (RazaoSocial)s.RazaoSocialTomador)
            .Build();

        InformacoesIbsCbs ibsCbs = InformacoesIbsCbsBuilder.New()
            .SetUsoOuConsumoPessoal((NaoSim)false)
            .SetCodigoOperacaoFornecimento((CodigoOperacao)s.V2.CodigoOperacao)
            .SetClassificacaoTributaria((ClassificacaoTributaria)s.V2.ClassificacaoTributaria)
            .Build();

        Rps rps = RpsBuilder
            .New(
                inscricaoPrestador: (InscricaoMunicipal)s.InscricaoMunicipalPrestador,
                tipoRps: TipoRps.Rps,
                numeroRps: (Numero)s.V2.NumeroRps,
                discriminacao: (Discriminacao)s.V2.Discriminacao,
                serieRps: (SerieRps)s.V2.SerieRps)
            .SetNFe(
                dataEmissao: (DataXsd)DateTime.Today,
                tributacaoNFe: (TributacaoNfe)'T',
                exigibilidadeSuspensa: (NaoSim)false,
                pagamentoParceladoAntecipado: (NaoSim)false,
                statusNFe: StatusNfe.Normal)
            .SetServico(
                codigoServico: (CodigoServico)s.V2.CodigoServico,
                nbs: (CodigoNBS)s.V2.CodigoNbs)
            .SetIss(
                aliquota: (Aliquota)s.V2.Aliquota,
                issRetido: false)
            .SetIbsCbs(ibsCbs)
            .SetValorFinalCobrado((Valor)s.V2.ValorFinalCobrado)
            .SetLocalPrestacao((CodigoIbge)s.V2.CodigoMunicipioIbge)
            .SetTomador(tomador)
            .Build();

        PedidoEnvioLoteFactory loteFactory = fixture.GetService<PedidoEnvioLoteFactory>();
        PedidoEnvioLote pedidoLote = loteFactory.NewCnpj(
            cnpj: (Cnpj)s.CnpjPrestador,
            transacao: false,
            rpsList: [rps]);

        // Act
        IEnvioLoteRpsService service = fixture.GetService<IEnvioLoteRpsService>();
        RetornoEnvioLoteRps retorno = await service.SendAsync(pedidoLote, modoTeste: true);

        // Assert
        // Atualmente pela regra da prefeitura de São Paulo um prestador só pode estar
        // configurado para a versão 1 ou a versão 2. No meu caso, como a configuração está
        // apontando para a versão 1, o esperado é que o teste do V2 retorne um erro de
        // validação específico - conforme os Asserts abaixo demonstram.
        Assert.NotNull(retorno);
        Assert.Null(retorno.Alerta);
        Assert.Null(retorno.ChavesNFeRps);
        Assert.NotNull(retorno.Cabecalho);
        Assert.False(retorno.Cabecalho.Sucesso); // esperado falso devido à configuração do prestador para V1
        Assert.NotNull(retorno.Cabecalho.InformacoesLote);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente);
        Assert.Null(retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente.Cpf);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente.Cnpj);
        Assert.Equal((Cnpj)s.CnpjPrestador, (Cnpj)retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente.Cnpj);
        Assert.Equal(DateTime.Today.Date, retorno.Cabecalho.InformacoesLote.DataEnvioLote.Date);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.InscricaoPrestador);
        Assert.Equal((InscricaoMunicipal)s.InscricaoMunicipalPrestador, (InscricaoMunicipal)retorno.Cabecalho.InformacoesLote.InscricaoPrestador);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.NumeroLote);
        Assert.Equal(0, retorno.Cabecalho.InformacoesLote.QtdNotasProcessadas); //esperado 0 devido à falha de validação
        Assert.Equal(0, retorno.Cabecalho.InformacoesLote.ValorTotalServicos); // esperado 0 devido à falha de validação
        Assert.NotNull(retorno.Erro); //esperado erro devido à configuração do prestador para V1, que não aceita campos exclusivos do V2
        EventoRetorno erro = Assert.Single(retorno.Erro);
        Assert.Null(erro.ChaveNFe);
        Assert.NotNull(erro.ChaveRPS);
        Assert.NotNull(erro.ChaveRPS.InscricaoPrestador);
        Assert.Equal((InscricaoMunicipal)s.InscricaoMunicipalPrestador, (InscricaoMunicipal)erro.ChaveRPS.InscricaoPrestador);
        Assert.NotNull(erro.ChaveRPS.NumeroRps);
        Assert.Equal(s.V2.NumeroRps, long.Parse(erro.ChaveRPS.NumeroRps));
        Assert.NotNull(erro.ChaveRPS.SerieRps);
        Assert.Equal(s.V2.SerieRps, erro.ChaveRPS.SerieRps);
        Assert.Equal(641, erro.Codigo);
        Assert.Contains("ser utilizado o leiaute 1", erro.Descricao);
    }
}