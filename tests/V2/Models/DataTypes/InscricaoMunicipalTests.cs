using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="InscricaoMunicipal"/> (V2 — long, 12 dígitos).
/// </summary>
public class InscricaoMunicipalTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new InscricaoMunicipal().ToString());

    // ============================================
    // Zero-padding para 12 dígitos
    // ============================================

    [Fact]
    public void Constructor_ComValorCompleto_NaoPrecisaDePadding() =>
        Assert.Equal("000039616924", new InscricaoMunicipal(39_616_924).ToString());

    [Fact]
    public void Constructor_ComValorMinimo_PadParaDozeDigitos() =>
        Assert.Equal("000000000001", new InscricaoMunicipal(1).ToString());

    [Fact]
    public void Constructor_ComUmDigito_ProduzeStringDeDozeChars() =>
        Assert.Equal(12, new InscricaoMunicipal(5).ToString()!.Length);

    [Fact]
    public void Constructor_ComValorMaximoValido_ArmazenaCorretamente() =>
        Assert.Equal("999999999999", new InscricaoMunicipal(999_999_999_999).ToString());

    [Fact]
    public void Constructor_ComZero_LancaArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InscricaoMunicipal(0L));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AcimaDoMaximo_LancaArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InscricaoMunicipal(1_000_000_000_000L));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_ComNegativo_LancaArgumentException() =>
        Assert.Throws<ArgumentException>(() => new InscricaoMunicipal(-1L));

    // ============================================
    // Construtor string
    // ============================================

    [Fact]
    public void Constructor_ComString_AplicaPaddingCorretamente() =>
        Assert.Equal("000000000001", new InscricaoMunicipal("1").ToString());

    [Fact]
    public void Constructor_ComStringFormatada_RemoveFormatacao() =>
        Assert.Equal("000039616924", new InscricaoMunicipal("39.616.924").ToString());

    [Fact]
    public void Constructor_ComStringNaoNumerica_LancaArgumentException() =>
        Assert.Throws<ArgumentException>(() => new InscricaoMunicipal("ABC-IM"));

    // ============================================
    // Factory methods e operadores de cast
    // ============================================

    [Fact]
    public void FromString_ProducesMesmoResultadoQueConstructor() =>
        Assert.Equal(new InscricaoMunicipal("39616924").ToString(), InscricaoMunicipal.FromString("39616924").ToString());

    [Fact]
    public void FromInt64_ProducesMesmoResultadoQueConstructor() =>
        Assert.Equal(new InscricaoMunicipal(39_616_924).ToString(), InscricaoMunicipal.FromInt64(39_616_924).ToString());

    [Fact]
    public void ExplicitCastFromString_ProducesMesmoResultadoQueFromString() =>
        Assert.Equal(InscricaoMunicipal.FromString("39616924").ToString(), ((InscricaoMunicipal)"39616924").ToString());

    [Fact]
    public void ExplicitCastFromLong_ProducesMesmoResultadoQueFromInt64() =>
        Assert.Equal(InscricaoMunicipal.FromInt64(39_616_924).ToString(), ((InscricaoMunicipal)39_616_924L).ToString());

    // ============================================
    // Equals / GetHashCode / sealed
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(InscricaoMunicipal).IsSealed);

    [Fact]
    public void Equals_MesmoValor_RetornaTrue() =>
        Assert.Equal(new InscricaoMunicipal(39_616_924), new InscricaoMunicipal(39_616_924));

    [Fact]
    public void Equals_ValorDiferente_RetornaFalse() =>
        Assert.NotEqual(new InscricaoMunicipal(39_616_924), new InscricaoMunicipal(12_345_678));

    // ============================================
    // Serialização / desserialização XML
    // ============================================

    [Fact]
    public void XmlRoundTrip_PreservaValor()
    {
        // Arrange
        var original = new InscricaoMunicipal(39_616_924);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(InscricaoMunicipal));

        // Act
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, original);
        using var sr = new System.IO.StringReader(sw.ToString());
        var deserialized = (InscricaoMunicipal?)serializer.Deserialize(sr);

        // Assert
        Assert.Equal("000039616924", deserialized?.ToString());
    }
}
