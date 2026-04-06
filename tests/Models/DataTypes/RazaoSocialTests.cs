using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class RazaoSocialTests
{
    [Fact]
    public void RazaoSocial_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new RazaoSocial().ToString());

    [Fact]
    public void RazaoSocial_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Empresa Exemplo Ltda.", new RazaoSocial("Empresa Exemplo Ltda.").ToString());

    [Fact]
    public void RazaoSocial_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('R', 75);
        Assert.Equal(value, new RazaoSocial(value).ToString());
    }

    [Fact]
    public void RazaoSocial_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RazaoSocial(new string('R', 76)));

    [Fact]
    public void RazaoSocial_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new RazaoSocial(string.Empty));

    [Fact]
    public void RazaoSocial_Constructor_WithAmpersand_NormalizesCorrectly() =>
        Assert.Equal("Empresa A &amp; B Ltda.", new RazaoSocial("Empresa A & B Ltda.").ToString());

    [Fact]
    public void RazaoSocial_IsSealed() =>
        Assert.True(typeof(RazaoSocial).IsSealed);

    [Fact]
    public void RazaoSocial_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new RazaoSocial("Acme Corp."), new RazaoSocial("Acme Corp."));

    [Fact]
    public void RazaoSocial_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new RazaoSocial("Acme Corp."), new RazaoSocial("Beta Corp."));

    [Fact]
    public void RazaoSocial_GetHashCode_SameValue_ReturnsEqualHash() =>
        Assert.Equal(new RazaoSocial("Acme Corp.").GetHashCode(), new RazaoSocial("Acme Corp.").GetHashCode());
}