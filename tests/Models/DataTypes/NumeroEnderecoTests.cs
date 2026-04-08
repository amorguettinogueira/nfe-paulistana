using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class NumeroEnderecoTests
{
    [Fact]
    public void NumeroEndereco_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new NumeroEndereco().ToString());

    [Fact]
    public void NumeroEndereco_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("1500", new NumeroEndereco("1500").ToString());

    [Fact]
    public void NumeroEndereco_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('9', 10);
        Assert.Equal(value, new NumeroEndereco(value).ToString());
    }

    [Fact]
    public void NumeroEndereco_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new NumeroEndereco(new string('9', 11)));

    [Fact]
    public void NumeroEndereco_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new NumeroEndereco(string.Empty));

    [Fact]
    public void NumeroEndereco_Constructor_WithApostrophe_NormalizesCorrectly() =>
        Assert.Equal("S/N&apos;", new NumeroEndereco("S/N'").ToString());

    [Fact]
    public void NumeroEndereco_IsSealed() =>
        Assert.True(typeof(NumeroEndereco).IsSealed);

    [Fact]
    public void NumeroEndereco_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new NumeroEndereco("100"), new NumeroEndereco("100"));

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(NumeroEndereco.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(NumeroEndereco.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(NumeroEndereco.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("1500", NumeroEndereco.ParseIfPresent("1500")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => NumeroEndereco.ParseIfPresent(new string('9', 11)));
}