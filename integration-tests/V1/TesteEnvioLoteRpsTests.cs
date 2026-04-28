using Nfe.Paulistana.IntegrationTests.Configuration;
using Nfe.Paulistana.IntegrationTests.Fixtures;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;

namespace Nfe.Paulistana.IntegrationTests.V1;

/// <summary>
/// Testes de integração Tier 3 para o pipeline completo de envio de RPS V1:
/// constrói um RPS com dados reais, assina com certificado A1 pessoal
/// e envia ao endpoint <c>TesteEnvioLoteRPS</c> da Prefeitura de São Paulo
/// (<c>modoTeste: true</c> — o servidor valida sem emitir NF-e definitiva).
/// Requer User Secrets configurados — pulados automaticamente em sua ausência.
/// </summary>
public sealed class TesteEnvioLoteRpsTests(LocalIntegrationFixture fixture)
    : IClassFixture<LocalIntegrationFixture>
{
    // ============================================
    // TesteEnvioLoteRPS — pipeline completo V1
    // ============================================

    [SkippableFact]
    public async Task TesteEnvioLoteRps_RpsComDadosReais_ServidorRespondeSemExcecao()
    {
        Skip.IfNot(fixture.IsConfigured,
            "User Secrets não configurados — execute 'dotnet user-secrets set' no projeto integration-tests.");

        // Arrange
        IntegrationTestSettings s = fixture.Settings;

        Tomador tomador = TomadorBuilder
            .NewCnpj((Cnpj)s.CnpjTomador, (RazaoSocial)s.RazaoSocialTomador)
            .Build();

        Rps rps = RpsBuilder
            .New(
                inscricaoPrestador: (InscricaoMunicipal)s.InscricaoMunicipalPrestador,
                tipoRps: TipoRps.Rps,
                numeroRps: (Numero)s.V1.NumeroRps,
                discriminacao: (Discriminacao)s.V1.Discriminacao,
                serieRps: (SerieRps)s.V1.SerieRps)
            .SetNFe(
                dataEmissao: (DataXsd)DateTime.Today,
                tributacaoNFe: (TributacaoNfe)'T',
                statusNFe: StatusNfe.Normal)
            .SetServico(
                codigoServico: (CodigoServico)s.V1.CodigoServico,
                valorServicos: (Valor)s.V1.ValorServicos)
            .SetIss(
                aliquota: (Aliquota)s.V1.Aliquota,
                issRetido: false)
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
        Assert.NotNull(retorno);
        Assert.Null(retorno.Alerta);
        Assert.Null(retorno.ChavesNFeRps);
        Assert.Null(retorno.Erro);
        Assert.NotNull(retorno.Cabecalho);
        Assert.True(retorno.Cabecalho.Sucesso);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente);
        Assert.Null(retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente.Cpf);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente.Cnpj);
        Assert.Equal((Cnpj)s.CnpjPrestador, (Cnpj)retorno.Cabecalho.InformacoesLote.CpfCnpjRemetente.Cnpj);
        Assert.Equal(DateTime.Today.Date, retorno.Cabecalho.InformacoesLote.DataEnvioLote.Date);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.InscricaoPrestador);
        Assert.Equal((InscricaoMunicipal)s.InscricaoMunicipalPrestador, (InscricaoMunicipal)retorno.Cabecalho.InformacoesLote.InscricaoPrestador);
        Assert.NotNull(retorno.Cabecalho.InformacoesLote.NumeroLote);
        Assert.Equal(1, retorno.Cabecalho.InformacoesLote.QtdNotasProcessadas);
        Assert.Equal(s.V1.ValorServicos, retorno.Cabecalho.InformacoesLote.ValorTotalServicos);
    }
}