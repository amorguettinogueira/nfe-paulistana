using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class BairroTests
{
    [Fact]
    public void Bairro_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Bairro().ToString());

    [Fact]
    public void Bairro_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Vila Madalena", new Bairro("Vila Madalena").ToString());

    [Fact]
    public void Bairro_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('A', 30);
        Assert.Equal(value, new Bairro(value).ToString());
    }

    [Fact]
    public void Bairro_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Bairro(new string('A', 31)));

    [Fact]
    public void Bairro_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Bairro(string.Empty));

    [Fact]
    public void Bairro_Constructor_WithWhitespace_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Bairro("   "));

    [Fact]
    public void Bairro_Constructor_WithAmpersand_NormalizesCorrectly() =>
        Assert.Equal("Pinheiros &amp; Outros", new Bairro("Pinheiros & Outros").ToString());

    [Fact]
    public void Bairro_FromString_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Centro", CodigoEndPostal.FromString("Centro").ToString());

    [Fact]
    public void Bairro_CastOperator_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Centro", ((CodigoEndPostal)"Centro").ToString());

    [Fact]
    public void Bairro_IsSealed() =>
        Assert.True(typeof(CodigoEndPostal).IsSealed);

    [Fact]
    public void Bairro_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Bairro("Centro"), new Bairro("Centro"));

    [Fact]
    public void Bairro_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Bairro("Centro"), new Bairro("Sul"));

    [Fact]
    public void Bairro_GetHashCode_SameValue_ReturnsEqualHash() =>
        Assert.Equal(new Bairro("Centro").GetHashCode(), new Bairro("Centro").GetHashCode());
}