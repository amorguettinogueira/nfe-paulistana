using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class CodigoVerificacaoTests
{
    // ============================================
    // CodigoVerificacao(string) — válido
    // ============================================

    [Fact]
    public void Constructor_WithExactlyEightChars_StoresCorrectly() =>
        Assert.Equal("ABCDEFGH", new CodigoVerificacao("ABCDEFGH").ToString());

    [Fact]
    public void Constructor_WithEightNumericChars_StoresCorrectly() =>
        Assert.Equal("12345678", new CodigoVerificacao("12345678").ToString());

    // ============================================
    // CodigoVerificacao(string) — inválido
    // ============================================

    [Fact]
    public void Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new CodigoVerificacao(string.Empty));

    [Fact]
    public void Constructor_WithFewerThanEightChars_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new CodigoVerificacao("ABCDEFG"));

    [Fact]
    public void Constructor_WithMoreThanEightChars_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new CodigoVerificacao("ABCDEFGHI"));

    [Fact]
    public void Constructor_WithNull_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new CodigoVerificacao(null!));

    // ============================================
    // FromString / operador de cast
    // ============================================

    [Fact]
    public void FromString_WithValidValue_ReturnsInstance() =>
        Assert.Equal("12345678", CodigoVerificacao.FromString("12345678").ToString());

    [Fact]
    public void CastOperator_WithValidValue_ReturnsInstance() =>
        Assert.Equal("ABCDEFGH", ((CodigoVerificacao)"ABCDEFGH").ToString());

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CodigoVerificacao).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new CodigoVerificacao("12345678"), new CodigoVerificacao("12345678"));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new CodigoVerificacao("12345678"), new CodigoVerificacao("87654321"));

    [Fact]
    public void GetHashCode_SameValue_ReturnsEqualHash() =>
        Assert.Equal(new CodigoVerificacao("12345678").GetHashCode(), new CodigoVerificacao("12345678").GetHashCode());

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(CodigoVerificacao.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(CodigoVerificacao.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(CodigoVerificacao.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("ABCDEFGH", CodigoVerificacao.ParseIfPresent("ABCDEFGH")!.ToString());

    [Fact]
    public void ParseIfPresent_FewerThanEightCharsInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoVerificacao.ParseIfPresent("ABCDEFG"));

    [Fact]
    public void ParseIfPresent_MoreThanEightCharsInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoVerificacao.ParseIfPresent("ABCDEFGHI"));

    // ============================================
    // Serialização / desserialização XML
    // ============================================

    [Fact]
    public void XmlRoundTrip_ValidValue_PreservesValue()
    {
        // Arrange
        var original = new CodigoVerificacao("ABCDEFGH");
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CodigoVerificacao));

        // Act
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, original);
        using var sr = new System.IO.StringReader(sw.ToString());
        var deserialized = (CodigoVerificacao?)serializer.Deserialize(sr);

        // Assert
        Assert.Equal("ABCDEFGH", deserialized?.ToString());
    }

    [Fact]
    public void DefaultConstructor_InstanciaVazia_ToStringRetornaNull() =>
        Assert.Null(new CodigoVerificacao().ToString());
}
