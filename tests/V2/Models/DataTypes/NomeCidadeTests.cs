using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para a classe <see cref="NomeCidade"/>.
/// </summary>
public sealed class NomeCidadeTests
{
    [Fact]
    public void Constructor_ValidValue_ShouldCreateInstance()
    {
        // Arrange
        var value = "São Paulo";

        // Act
        var nomeCidade = new NomeCidade(value);

        // Assert
        Assert.Equal("São Paulo", nomeCidade.ToString());
    }

    [Fact]
    public void Constructor_EmptyValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var act = () => new NomeCidade(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_TooLongValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = new string('A', 61);

        // Act
        var act = () => new NomeCidade(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "Campinas";

        // Act
        var nomeCidade = NomeCidade.FromString(value);

        // Assert
        Assert.Equal("Campinas", nomeCidade.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "Ribeirão Preto";

        // Act
        var nomeCidade = (NomeCidade)value;

        // Assert
        Assert.Equal("Ribeirão Preto", nomeCidade.ToString());
    }

    [Fact]
    public void Constructor_NullValue_ShouldThrowArgumentException()
    {
        // Arrange
        string? value = null;

        // Act
        var act = () => new NomeCidade(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WhitespaceOnly_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "   ";

        // Act
        var act = () => new NomeCidade(value);

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
        var nomeCidade = new NomeCidade(value);

        // Assert
        Assert.Equal(expected, nomeCidade.ToString());
    }

    [Fact]
    public void Constructor_ExactlyMaxLength_ShouldCreateInstance()
    {
        // Arrange
        var value = new string('X', 60);

        // Act
        var nomeCidade = new NomeCidade(value);

        // Assert
        Assert.Equal(value, nomeCidade.ToString());
    }

    [Fact]
    public void Constructor_TrimmedWhitespace_ShouldTrimAndCreateInstance()
    {
        // Arrange
        var value = "   Campinas   ";

        // Act
        var nomeCidade = new NomeCidade(value);

        // Assert
        Assert.Equal("Campinas", nomeCidade.ToString());
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var value = "CidadeX";
        var n1 = new NomeCidade(value);
        var n2 = new NomeCidade(value);

        // Assert
        Assert.Equal(n1, n2);
        Assert.Equal(n1.GetHashCode(), n2.GetHashCode());
    }

    [Fact]
    public void XmlSerialization_RoundTrip_ShouldPreserveValue()
    {
        // Arrange
        var value = "Cidade-XML-Teste";
        var nomeCidade = new NomeCidade(value);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(NomeCidade));
        using var ms = new System.IO.MemoryStream();

        // Act
        serializer.Serialize(ms, nomeCidade);
        ms.Position = 0;
        var deserialized = (NomeCidade)serializer.Deserialize(ms)!;

        // Assert
        Assert.Equal(value, deserialized.ToString());
        Assert.Equal(nomeCidade, deserialized);
    }
}