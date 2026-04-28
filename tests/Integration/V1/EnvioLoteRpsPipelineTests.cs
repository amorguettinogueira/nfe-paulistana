using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Tests.Integration.V1;

/// <summary>
/// Testes de integração estrutural para o pipeline de envio de lote de RPS V1:
/// construção de múltiplos <see cref="V1.Models.Domain.Rps"/>, assinatura individual de cada elemento
/// e validação do lote completo contra o XSD embutido — sem chamadas de rede.
/// </summary>
public sealed class EnvioLoteRpsPipelineTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    // ============================================
    // Validação XSD — pipeline completo
    // ============================================

    [Fact]
    public void PedidoEnvioLote_RpsUnico_XmlValidoPeloXsd()
    {
        // Arrange
        var lote = new PedidoEnvioLoteFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao()]);

        // Act
        bool valido = ValidationHelper.Validate(lote, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    [Fact]
    public void PedidoEnvioLote_TresRps_XmlValidoPeloXsd()
    {
        // Arrange — três RPS com valores distintos para garantir que o XSD valida o lote inteiro
        var rps1 = RpsTestFactory.Padrao(valorServicos: 500m);
        var rps2 = RpsTestFactory.Padrao(valorServicos: 1000m);
        var rps3 = RpsTestFactory.Padrao(valorServicos: 1500m);

        var lote = new PedidoEnvioLoteFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [rps1, rps2, rps3]);

        // Act
        bool valido = ValidationHelper.Validate(lote, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    // ============================================
    // Estrutura da assinatura digital
    // ============================================

    [Fact]
    public void PedidoEnvioLote_TresRps_CadaRpsContemSuaPropriaAssinatura()
    {
        // Arrange
        var lote = new PedidoEnvioLoteFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false,
            [
                RpsTestFactory.Padrao(valorServicos: 100m),
                RpsTestFactory.Padrao(valorServicos: 200m),
                RpsTestFactory.Padrao(valorServicos: 300m),
            ]);

        // Act — conta as ocorrências do elemento <Signature no XML do lote
        string xml = lote.SignedXmlContent!;
        int totalAssinaturas = xml.Split("<Signature").Length - 1;

        // Assert — cada RPS é assinado individualmente, portanto devem existir 3 assinaturas
        Assert.Equal(3, totalAssinaturas);
    }
}