using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Tests.Xml;

/// <summary>
/// Testes unitários para <see cref="ValidationHelper"/>:
/// guard clause de argumento nulo, ramo com <see cref="ISignedXmlFile.SignedXmlContent"/>
/// preenchido, ramo sem <c>SignedXmlContent</c> e acúmulo de erros de validação XSD.
/// </summary>
public class ValidationHelperTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    private PedidoEnvioLote CriarLoteAssinado()
    {
        var factory = new PedidoEnvioLoteFactory(fixture.Certificado);
        return factory.NewCpf((Cpf)Tests.Helpers.TestConstants.ValidCpf, false, [RpsTestFactory.Padrao()]);
    }

    // ============================================
    // Guard clauses
    // ============================================

    [Fact]
    public void Validate_ObjetoNulo_ThrowsArgumentNullException()
    {
        PedidoEnvioLote? pedido = null;

        _ = Assert.Throws<ArgumentNullException>(() => ValidationHelper.Validate(pedido!, out _));
    }

    // ============================================
    // Ramo com SignedXmlContent preenchido
    // ============================================

    [Fact]
    public void Validate_LoteComSignedXmlContent_RetornaTrue()
    {
        PedidoEnvioLote pedido = CriarLoteAssinado();

        bool resultado = ValidationHelper.Validate(pedido, out string? erro);

        Assert.True(resultado);
        Assert.Null(erro);
    }

    // ============================================
    // Ramo sem SignedXmlContent (serialização direta)
    // ============================================

    [Fact]
    public void Validate_LoteSemSignedXmlContent_RetornaFalseComErro()
    {
        // O XSD de PedidoEnvioLoteRPS exige o elemento <Signature>.
        // O XML re-serializado sem assinatura não satisfaz esse requisito,
        // documentando que o lote deve ser assinado antes de ser validado.
        PedidoEnvioLote pedido = CriarLoteAssinado();
        pedido.SignedXmlContent = null;

        bool resultado = ValidationHelper.Validate(pedido, out string? erro);

        Assert.False(resultado);
        Assert.NotNull(erro);
    }

    // ============================================
    // Acúmulo de erros de validação XSD
    // ============================================

    [Fact]
    public void Validate_SignedXmlContentInvalido_RetornaFalseComErro()
    {
        PedidoEnvioLote pedido = CriarLoteAssinado();
        pedido.SignedXmlContent = "<PedidoEnvioLoteRPS xmlns=\"http://www.prefeitura.sp.gov.br/nfe\"/>";  // Ausência de Cabecalho/RPS obrigatórios

        bool resultado = ValidationHelper.Validate(pedido, out string? erro);

        Assert.False(resultado);
        Assert.NotNull(erro);
    }
}