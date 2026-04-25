using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="DetalheConsultaNFe"/> (V1).
/// </summary>
public sealed class DetalheConsultaNFeTests
{
    private static InscricaoMunicipal CriarInscricao() =>
        new(39_616_924);

    private static ChaveRps CriarChaveRps() =>
        new(CriarInscricao(), new SerieRps("BB"), new Numero(1));

    private static ChaveNfe CriarChaveNfe() =>
        new(CriarInscricao(), new Numero(1), new CodigoVerificacao("ABCDEFGH"));

    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_PropriedadesNulas()
    {
        var detalhe = new DetalheConsultaNFe();

        Assert.Null(detalhe.ChaveRps);
        Assert.Null(detalhe.ChaveNfe);
    }

    // ============================================
    // Construtor com ChaveRps
    // ============================================

    [Fact]
    public void Constructor_ComChaveRps_DefineChaveRps()
    {
        var chave = CriarChaveRps();

        var detalhe = new DetalheConsultaNFe(chave);

        Assert.Same(chave, detalhe.ChaveRps);
    }

    [Fact]
    public void Constructor_ComChaveRps_DeixaChaveNfeNula()
    {
        var detalhe = new DetalheConsultaNFe(CriarChaveRps());

        Assert.Null(detalhe.ChaveNfe);
    }

    // ============================================
    // Construtor com ChaveNfe
    // ============================================

    [Fact]
    public void Constructor_ComChaveNfe_DefineChaveNfe()
    {
        var chave = CriarChaveNfe();

        var detalhe = new DetalheConsultaNFe(chave);

        Assert.Same(chave, detalhe.ChaveNfe);
    }

    [Fact]
    public void Constructor_ComChaveNfe_DeixaChaveRpsNula()
    {
        var detalhe = new DetalheConsultaNFe(CriarChaveNfe());

        Assert.Null(detalhe.ChaveRps);
    }

    // ============================================
    // Mutabilidade via propriedades
    // ============================================

    [Fact]
    public void ChaveRps_QuandoAtribuida_RetornaMesmaReferencia()
    {
        var chave = CriarChaveRps();
        var detalhe = new DetalheConsultaNFe { ChaveRps = chave };

        Assert.Same(chave, detalhe.ChaveRps);
    }

    [Fact]
    public void ChaveNfe_QuandoAtribuida_RetornaMesmaReferencia()
    {
        var chave = CriarChaveNfe();
        var detalhe = new DetalheConsultaNFe { ChaveNfe = chave };

        Assert.Same(chave, detalhe.ChaveNfe);
    }

    // ============================================
    // Tipo
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(DetalheConsultaNFe).IsSealed);
}