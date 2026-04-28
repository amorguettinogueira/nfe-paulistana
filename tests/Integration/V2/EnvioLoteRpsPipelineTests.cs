using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Tests.Integration.V2;

/// <summary>
/// Testes de integração estrutural para o pipeline de envio de lote de RPS V2:
/// construção de múltiplos <see cref="V2.Models.Domain.Rps"/>, assinatura individual de cada elemento
/// e validação do lote completo contra o XSD embutido — sem chamadas de rede.
/// </summary>
public sealed class EnvioLoteRpsPipelineTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private static readonly InformacoesIbsCbs IbsCbsPadrao =
        InformacoesIbsCbsBuilder.New()
            .SetUsoOuConsumoPessoal(new NaoSim(false))
            .SetCodigoOperacaoFornecimento(new CodigoOperacao("010101"))
            .SetClassificacaoTributaria(new ClassificacaoTributaria("010101"))
            .Build();

    // ============================================
    // Validação XSD — pipeline completo
    // ============================================

    [Fact]
    public void PedidoEnvioLote_RpsUnico_XmlValidoPeloXsd()
    {
        // Arrange
        var lote = new PedidoEnvioLoteFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false,
                [RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao)]);

        // Act
        bool valido = ValidationHelper.Validate(lote, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    [Fact]
    public void PedidoEnvioLote_TresRps_XmlValidoPeloXsd()
    {
        // Arrange — três RPS com os campos exclusivos da V2 para validar o lote inteiro
        var lote = new PedidoEnvioLoteFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false,
            [
                RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao),
                RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao),
                RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao),
            ]);

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
                RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao),
                RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao),
                RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao),
            ]);

        // Act — conta as ocorrências do elemento <Signature no XML do lote
        string xml = lote.SignedXmlContent!;
        int totalAssinaturas = xml.Split("<Signature").Length - 1;

        // Assert — cada RPS é assinado individualmente, portanto devem existir 3 assinaturas
        Assert.Equal(3, totalAssinaturas);
    }
}