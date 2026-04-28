using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Tests.Integration.V2;

/// <summary>
/// Testes de integração estrutural para o pipeline de envio de RPS unitário V2:
/// construção completa via <see cref="RpsBuilder"/>, assinatura com certificado auto-assinado
/// e validação do XML resultante contra o XSD embutido — sem chamadas de rede.
/// Verifica também a presença dos campos exclusivos da V2 (CodigoNBS, IBS/CBS).
/// </summary>
public sealed class EnvioRpsPipelineTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
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
    public void PedidoEnvio_RpsMinimo_XmlValidoPeloXsd()
    {
        // Arrange
        var rps = RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao);
        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, rps);

        // Act
        bool valido = ValidationHelper.Validate(pedido, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    [Fact]
    public void PedidoEnvio_ComTomadorCpf_XmlValidoPeloXsd()
    {
        // Arrange
        var tomador = TomadorBuilder
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf)
            .Build();

        var rps = RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao, tomador: tomador);
        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, rps);

        // Act
        bool valido = ValidationHelper.Validate(pedido, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    // ============================================
    // Estrutura da assinatura digital
    // ============================================

    [Fact]
    public void PedidoEnvio_XmlAssinado_ContemElementosDeAssinaturaDigital()
    {
        // Arrange
        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao));

        // Act
        string xml = pedido.SignedXmlContent!;

        // Assert — presença dos três elementos obrigatórios da assinatura XML
        Assert.Contains("<Signature", xml);
        Assert.Contains("<DigestValue>", xml);
        Assert.Contains("<SignatureValue>", xml);
    }

    // ============================================
    // Campos exclusivos da V2
    // ============================================

    [Fact]
    public void PedidoEnvio_XmlAssinado_ContemCamposExclusivosV2()
    {
        // Arrange
        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao(ibsCbs: IbsCbsPadrao));

        // Act
        string xml = pedido.SignedXmlContent!;

        // Assert — NBS e IBSCBS são elementos obrigatórios no schema V2, ausentes no V1
        Assert.Contains("<NBS>", xml);
        Assert.Contains("<IBSCBS>", xml);
    }
}