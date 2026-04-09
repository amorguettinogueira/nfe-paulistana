using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="DetalheCancelamentoNFe"/> (V2).
/// </summary>
public sealed class DetalheCancelamentoNFeTests
{
    private static ChaveNfe CriarChaveNfe() =>
        new(new InscricaoMunicipal(39_616_924L), new Numero(1), new CodigoVerificacao("ABCDEFGH"));

    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_PropriedadesNulas()
    {
        var detalhe = new DetalheCancelamentoNFe();

        Assert.Null(detalhe.ChaveNfe);
        Assert.Null(detalhe.AssinaturaCancelamento);
    }

    // ============================================
    // Construtor com ChaveNfe
    // ============================================

    [Fact]
    public void Constructor_ComChaveNfe_DefinePropriedade()
    {
        var chave = CriarChaveNfe();

        var detalhe = new DetalheCancelamentoNFe(chave);

        Assert.Same(chave, detalhe.ChaveNfe);
        Assert.Null(detalhe.AssinaturaCancelamento);
    }

    // ============================================
    // AssinaturaCancelamento
    // ============================================

    [Fact]
    public void AssinaturaCancelamento_QuandoDefinida_RetornaMesmosBytes()
    {
        var assinatura = new byte[] { 1, 2, 3, 4 };
        var detalhe = new DetalheCancelamentoNFe { AssinaturaCancelamento = assinatura };

        Assert.Equal(assinatura, detalhe.AssinaturaCancelamento);
    }

    // ============================================
    // ISignedElement — implementação explícita
    // ============================================

    [Fact]
    public void ISignedElement_Assinatura_LeSameReferencia()
    {
        var assinatura = new byte[] { 10, 20, 30 };
        var detalhe = new DetalheCancelamentoNFe { AssinaturaCancelamento = assinatura };

        Assert.Equal(assinatura, ((ISignedElement)detalhe).Assinatura);
    }

    [Fact]
    public void ISignedElement_Assinatura_Set_PropagaParaAssinaturaCancelamento()
    {
        var assinatura = new byte[] { 7, 8, 9 };
        ISignedElement detalhe = new DetalheCancelamentoNFe();

        detalhe.Assinatura = assinatura;

        Assert.Equal(assinatura, ((DetalheCancelamentoNFe)detalhe).AssinaturaCancelamento);
    }

    // ============================================
    // Tipo
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(DetalheCancelamentoNFe).IsSealed);

    [Fact]
    public void ImplementaISignedElement() =>
        Assert.True(typeof(DetalheCancelamentoNFe).IsAssignableTo(typeof(ISignedElement)));
}
