using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class LogradouroTests
{
    [Fact]
    public void Logradouro_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Logradouro().ToString());

    [Fact]
    public void Logradouro_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Rua das Flores", new Logradouro("Rua das Flores").ToString());

    [Fact]
    public void Logradouro_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('L', 50);
        Assert.Equal(value, new Logradouro(value).ToString());
    }

    [Fact]
    public void Logradouro_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Logradouro(new string('L', 51)));

    [Fact]
    public void Logradouro_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Logradouro(string.Empty));

    [Fact]
    public void Logradouro_Constructor_WithDoubleQuote_NormalizesCorrectly() =>
        Assert.Equal("Rua &quot;das&quot; Flores", new Logradouro("Rua \"das\" Flores").ToString());

    [Fact]
    public void Logradouro_IsSealed() =>
        Assert.True(typeof(Logradouro).IsSealed);

    [Fact]
    public void Logradouro_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Logradouro("Av. Paulista"), new Logradouro("Av. Paulista"));

    [Fact]
    public void Logradouro_GetHashCode_SameValue_ReturnsEqualHash() =>
        Assert.Equal(new Logradouro("Av. Paulista").GetHashCode(), new Logradouro("Av. Paulista").GetHashCode());
}