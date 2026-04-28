using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Tests.Integration.V1;

/// <summary>
/// Testes de integração estrutural para o pipeline de envio de RPS unitário V1:
/// construção completa via <see cref="RpsBuilder"/>, assinatura com certificado auto-assinado
/// e validação do XML resultante contra o XSD embutido — sem chamadas de rede.
/// </summary>
public sealed class EnvioRpsPipelineTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    // ============================================
    // Validação XSD — pipeline completo
    // ============================================

    [Fact]
    public void PedidoEnvio_RpsMinimo_XmlValidoPeloXsd()
    {
        // Arrange
        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao());

        // Act
        bool valido = ValidationHelper.Validate(pedido, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    [Fact]
    public void PedidoEnvio_ComTomadorCnpj_XmlValidoPeloXsd()
    {
        // Arrange
        var tomador = TomadorBuilder
            .NewCnpj((Cnpj)TestConstants.ValidCnpj, (RazaoSocial)"Empresa Tomadora Ltda")
            .Build();

        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao(tomador: tomador));

        // Act
        bool valido = ValidationHelper.Validate(pedido, out string? erro);

        // Assert
        Assert.True(valido, erro);
    }

    [Fact]
    public void PedidoEnvio_ViaCnpjPrestador_XmlValidoPeloXsd()
    {
        // Arrange — exercita o caminho NewCnpj do factory, complementar ao NewCpf
        var pedido = new PedidoEnvioFactory(fixture.Certificado)
            .NewCnpj((Cnpj)TestConstants.ValidCnpj, RpsTestFactory.Padrao());

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
            .NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao());

        // Act
        string xml = pedido.SignedXmlContent!;

        // Assert — presença dos três elementos obrigatórios da assinatura XML
        Assert.Contains("<Signature", xml);
        Assert.Contains("<DigestValue>", xml);
        Assert.Contains("<SignatureValue>", xml);
    }

    [Fact]
    public void PedidoEnvio_RpsComValoresDistintos_ProduzemXmlDistinto()
    {
        // Arrange — valores de serviço diferentes devem gerar digest e assinatura distintos
        var factory = new PedidoEnvioFactory(fixture.Certificado);

        var pedido100 = factory.NewCpf(
            (Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao(valorServicos: 100m));

        var pedido200 = factory.NewCpf(
            (Cpf)Tests.Helpers.TestConstants.ValidCpf, RpsTestFactory.Padrao(valorServicos: 200m));

        // Act & Assert
        Assert.NotEqual(pedido100.SignedXmlContent, pedido200.SignedXmlContent);
    }
}