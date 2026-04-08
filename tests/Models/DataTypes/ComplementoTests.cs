using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class ComplementoTests
{
    [Fact]
    public void Complemento_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Complemento().ToString());

    [Fact]
    public void Complemento_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Apto 42", new Complemento("Apto 42").ToString());

    [Fact]
    public void Complemento_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('B', 30);
        Assert.Equal(value, new Complemento(value).ToString());
    }

    [Fact]
    public void Complemento_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Complemento(new string('B', 31)));

    [Fact]
    public void Complemento_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Complemento(string.Empty));

    [Fact]
    public void Complemento_Constructor_WithGreaterThan_NormalizesCorrectly() =>
        Assert.Equal("Ap &gt; 5", new Complemento("Ap > 5").ToString());

    [Fact]
    public void Complemento_IsSealed() =>
        Assert.True(typeof(Complemento).IsSealed);

    [Fact]
    public void Complemento_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Complemento("Bloco A"), new Complemento("Bloco A"));

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(Complemento.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(Complemento.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(Complemento.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("Apto 42", Complemento.ParseIfPresent("Apto 42")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Complemento.ParseIfPresent(new string('B', 31)));
}