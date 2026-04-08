using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class FonteCargaTributariaTests
{
    [Fact]
    public void FonteCargaTributaria_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new FonteCargaTributaria().ToString());

    [Fact]
    public void FonteCargaTributaria_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("IBPT", new FonteCargaTributaria("IBPT").ToString());

    [Fact]
    public void FonteCargaTributaria_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('F', 10);
        Assert.Equal(value, new FonteCargaTributaria(value).ToString());
    }

    [Fact]
    public void FonteCargaTributaria_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new FonteCargaTributaria(new string('F', 11)));

    [Fact]
    public void FonteCargaTributaria_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new FonteCargaTributaria(string.Empty));

    [Fact]
    public void FonteCargaTributaria_IsSealed() =>
        Assert.True(typeof(FonteCargaTributaria).IsSealed);

    [Fact]
    public void FonteCargaTributaria_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new FonteCargaTributaria("IBPT"), new FonteCargaTributaria("IBPT"));

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(FonteCargaTributaria.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(FonteCargaTributaria.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(FonteCargaTributaria.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("IBPT", FonteCargaTributaria.ParseIfPresent("IBPT")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => FonteCargaTributaria.ParseIfPresent(new string('F', 11)));
}