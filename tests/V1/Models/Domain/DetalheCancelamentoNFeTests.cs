using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="DetalheCancelamentoNFe"/> (V1).
/// </summary>
public class DetalheCancelamentoNFeTests
{
    [Fact]
    public void DefaultConstructor_PropriedadesNulas()
    {
        var detalhe = new DetalheCancelamentoNFe();

        Assert.Null(detalhe.ChaveNfe);
        Assert.Null(detalhe.AssinaturaCancelamento);
    }

    [Fact]
    public void Constructor_ComChaveNfe_DefinePropriedade()
    {
        ChaveNfe chave = new(new InscricaoMunicipal(39_616_924), new Numero(1), new CodigoVerificacao("ABCDEFGH"));
        var detalhe = new DetalheCancelamentoNFe(chave);

        Assert.Same(chave, detalhe.ChaveNfe);
        Assert.Null(detalhe.AssinaturaCancelamento);
    }

    [Fact]
    public void AssinaturaCancelamento_QuandoDefinida_RetornaMesmosBytes()
    {
        var assinatura = new byte[] { 1, 2, 3, 4 };
        var detalhe = new DetalheCancelamentoNFe { AssinaturaCancelamento = assinatura };

        Assert.Equal(assinatura, detalhe.AssinaturaCancelamento);
    }

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

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(DetalheCancelamentoNFe).IsSealed);

    [Fact]
    public void ImplementaISignedElement() =>
        Assert.True(typeof(DetalheCancelamentoNFe).IsAssignableTo(typeof(ISignedElement)));
}