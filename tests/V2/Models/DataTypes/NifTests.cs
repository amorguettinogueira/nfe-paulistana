using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para a classe <see cref="Nif"/>.
/// </summary>
public sealed class NifTests
{
    [Fact]
    public void Constructor_ValidValue_ShouldCreateInstance()
    {
        // Arrange
        var value = "1234567890";

        // Act
        var nif = new Nif(value);

        // Assert
        Assert.Equal(value, nif.ToString());
    }

    [Fact]
    public void Constructor_EmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var act = () => new Nif(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_TooLongValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('A', 41);

        // Act
        var act = () => new Nif(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "987654321";

        // Act
        var nif = Nif.FromString(value);

        // Assert
        Assert.Equal(value, nif.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "1122334455";

        // Act
        var nif = (Nif)value;

        // Assert
        Assert.Equal(value, nif.ToString());
    }

    [Fact]
    public void Constructor_NullValue_ShouldThrowArgumentException()
    {
        // Arrange
        string? value = null;

        // Act
        var act = () => new Nif(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WhitespaceOnly_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "   ";

        // Act
        var act = () => new Nif(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_SpecialXmlCharacters_ShouldNormalize()
    {
        // Arrange
        var value = "A&B>C<D\"E'F";
        var expected = "A&amp;B&gt;C&lt;D&quot;E&apos;F";

        // Act
        var nif = new Nif(value);

        // Assert
        Assert.Equal(expected, nif.ToString());
    }

    [Fact]
    public void Constructor_ExactlyMaxLength_ShouldCreateInstance()
    {
        // Arrange
        var value = new string('X', 40);

        // Act
        var nif = new Nif(value);

        // Assert
        Assert.Equal(value, nif.ToString());
    }

    [Fact]
    public void Constructor_TrimmedWhitespace_ShouldTrimAndCreateInstance()
    {
        // Arrange
        var value = "   123456   ";

        // Act
        var nif = new Nif(value);

        // Assert
        Assert.Equal("123456", nif.ToString());
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var value = "ABC123";
        var nif1 = new Nif(value);
        var nif2 = new Nif(value);

        // Assert
        Assert.Equal(nif1, nif2);
        Assert.Equal(nif1.GetHashCode(), nif2.GetHashCode());
    }

    [Fact]
    public void XmlSerialization_RoundTrip_ShouldPreserveValue()
    {
        // Arrange
        var value = "NIF-XML-TEST";
        var nif = new Nif(value);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Nif));
        using var ms = new System.IO.MemoryStream();

        // Act
        serializer.Serialize(ms, nif);
        ms.Position = 0;
        var deserialized = (Nif)serializer.Deserialize(ms)!;

        // Assert
        Assert.Equal(value, deserialized.ToString());
        Assert.Equal(nif, deserialized);
    }
}