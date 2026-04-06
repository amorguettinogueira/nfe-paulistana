using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para a classe <see cref="NomeEvento"/>.
/// </summary>
public sealed class NomeEventoTests
{
    [Fact]
    public void Constructor_ValidValue_ShouldCreateInstance()
    {
        // Arrange
        var value = "Evento Cultural";

        // Act
        var nomeEvento = new NomeEvento(value);

        // Assert
        Assert.Equal("Evento Cultural", nomeEvento.ToString());
    }

    [Fact]
    public void Constructor_EmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var act = () => new NomeEvento(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_TooLongValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('A', 256);

        // Act
        var act = () => new NomeEvento(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "Show Musical";

        // Act
        var nomeEvento = NomeEvento.FromString(value);

        // Assert
        Assert.Equal("Show Musical", nomeEvento.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "Festival de Cinema";

        // Act
        var nomeEvento = (NomeEvento)value;

        // Assert
        Assert.Equal("Festival de Cinema", nomeEvento.ToString());
    }

    [Fact]
    public void Constructor_NullValue_ShouldThrowArgumentException()
    {
        // Arrange
        string? value = null;

        // Act
        var act = () => new NomeEvento(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WhitespaceOnly_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "   ";

        // Act
        var act = () => new NomeEvento(value);

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
        var nomeEvento = new NomeEvento(value);

        // Assert
        Assert.Equal(expected, nomeEvento.ToString());
    }

    [Fact]
    public void Constructor_ExactlyMaxLength_ShouldCreateInstance()
    {
        // Arrange
        var value = new string('X', 255);

        // Act
        var nomeEvento = new NomeEvento(value);

        // Assert
        Assert.Equal(value, nomeEvento.ToString());
    }

    [Fact]
    public void Constructor_TrimmedWhitespace_ShouldTrimAndCreateInstance()
    {
        // Arrange
        var value = "   Festival   ";

        // Act
        var nomeEvento = new NomeEvento(value);

        // Assert
        Assert.Equal("Festival", nomeEvento.ToString());
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var value = "EventoX";
        var n1 = new NomeEvento(value);
        var n2 = new NomeEvento(value);

        // Assert
        Assert.Equal(n1, n2);
        Assert.Equal(n1.GetHashCode(), n2.GetHashCode());
    }

    [Fact]
    public void XmlSerialization_RoundTrip_ShouldPreserveValue()
    {
        // Arrange
        var value = "Evento-XML-Teste";
        var nomeEvento = new NomeEvento(value);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(NomeEvento));
        using var ms = new System.IO.MemoryStream();

        // Act
        serializer.Serialize(ms, nomeEvento);
        ms.Position = 0;
        var deserialized = (NomeEvento)serializer.Deserialize(ms)!;

        // Assert
        Assert.Equal(value, deserialized.ToString());
        Assert.Equal(nomeEvento, deserialized);
    }
}