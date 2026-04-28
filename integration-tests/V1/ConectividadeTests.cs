using Nfe.Paulistana.IntegrationTests.Configuration;
using Nfe.Paulistana.IntegrationTests.Fixtures;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.V1.Services;

namespace Nfe.Paulistana.IntegrationTests.V1;

/// <summary>
/// Teste de conectividade básica com o webservice V1 da Prefeitura de São Paulo.
/// Usa <c>ConsultaCNPJ</c> — operação de leitura, sem efeitos colaterais — para verificar
/// que autenticação mTLS, endpoint e certificado estão operacionais.
/// Requer User Secrets configurados — pulados automaticamente em sua ausência.
/// </summary>
public sealed class ConectividadeTests(LocalIntegrationFixture fixture)
    : IClassFixture<LocalIntegrationFixture>
{
    // ============================================
    // ConsultaCNPJ — operação de leitura, sem efeito colateral
    // ============================================

    [SkippableFact]
    public async Task ConsultaCnpj_CnpjDoProprioContribuinte_ServidorRespondeSemExcecao()
    {
        Skip.IfNot(fixture.IsConfigured,
            "User Secrets não configurados — execute 'dotnet user-secrets set' no projeto integration-tests.");

        // Arrange
        IntegrationTestSettings s = fixture.Settings;
        var cnpj = (Cnpj)s.CnpjPrestador;
        PedidoConsultaCNPJFactory factory = fixture.GetService<PedidoConsultaCNPJFactory>();
        PedidoConsultaCNPJ pedido = factory.NewCnpj(cnpj, (CpfOrCnpj)cnpj);

        // Act
        IConsultaCNPJService service = fixture.GetService<IConsultaCNPJService>();
        RetornoConsultaCNPJ retorno = await service.SendAsync(pedido);

        // Assert
        Assert.NotNull(retorno);
        Assert.Null(retorno.Alerta);
        Assert.Null(retorno.Erro);
        Assert.NotNull(retorno.Cabecalho);
        Assert.True(retorno.Cabecalho.Sucesso);
        Assert.NotNull(retorno.Detalhe);
        Assert.NotEmpty(retorno.Detalhe);
        DetalheRetornoConsultaCNPJ detalhe = Assert.Single(retorno.Detalhe);
        Assert.True(detalhe.EmiteNFe);
        Assert.Equal((InscricaoMunicipal)s.InscricaoMunicipalPrestador, (InscricaoMunicipal)(int)detalhe.InscricaoMunicipal);
    }
}