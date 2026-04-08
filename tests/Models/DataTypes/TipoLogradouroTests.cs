using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class TipoLogradouroTests
{
    [Fact]
    public void TipoLogradouro_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new TipoLogradouro().ToString());

    [Fact]
    public void TipoLogradouro_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Rua", new TipoLogradouro("Rua").ToString());

    [Fact]
    public void TipoLogradouro_Constructor_WithMaxLengthValue_StoresCorrectly() =>
        Assert.Equal("Av.", new TipoLogradouro("Av.").ToString());

    [Fact]
    public void TipoLogradouro_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new TipoLogradouro("Rua2"));

    [Fact]
    public void TipoLogradouro_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new TipoLogradouro(string.Empty));

    [Fact]
    public void TipoLogradouro_IsSealed() =>
        Assert.True(typeof(TipoLogradouro).IsSealed);

    [Fact]
    public void TipoLogradouro_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new TipoLogradouro("Rua"), new TipoLogradouro("Rua"));

    [Fact]
    public void TipoLogradouro_GetHashCode_SameValue_ReturnsEqualHash() =>
        Assert.Equal(new TipoLogradouro("Av.").GetHashCode(), new TipoLogradouro("Av.").GetHashCode());

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(TipoLogradouro.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(TipoLogradouro.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(TipoLogradouro.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("Rua", TipoLogradouro.ParseIfPresent("Rua")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => TipoLogradouro.ParseIfPresent("Rua2"));
}