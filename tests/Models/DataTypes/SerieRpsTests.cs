using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class SerieRpsTests
{
    [Fact]
    public void SerieRps_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new SerieRps().ToString());

    [Fact]
    public void SerieRps_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("BB", new SerieRps("BB").ToString());

    [Fact]
    public void SerieRps_Constructor_WithMaxLengthValue_StoresCorrectly() =>
        Assert.Equal("BBBBB", new SerieRps("BBBBB").ToString());

    [Fact]
    public void SerieRps_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new SerieRps("BBBBBB"));

    [Fact]
    public void SerieRps_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new SerieRps(string.Empty));

    [Fact]
    public void SerieRps_IsSealed() =>
        Assert.True(typeof(SerieRps).IsSealed);

    [Fact]
    public void SerieRps_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new SerieRps("BB"), new SerieRps("BB"));

    [Fact]
    public void SerieRps_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new SerieRps("AA"), new SerieRps("BB"));
}