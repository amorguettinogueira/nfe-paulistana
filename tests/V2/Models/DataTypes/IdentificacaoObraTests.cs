using Nfe.Paulistana.V2.Models.DataTypes;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="IdentificacaoObra"/>.
/// </summary>
public sealed class IdentificacaoObraTests
{
    [Theory]
    [InlineData("123456789012345678901234567890")] // 30 chars
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234")] // 30 chars
    [InlineData("000000000000000000000000000000")] // 30 chars
    public void Constructor_ValidValue_ShouldSetValue(string value)
    {
        // Act
        var obra = new IdentificacaoObra(value);

        // Assert
        Assert.NotNull(obra);
        Assert.Equal(value, obra.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678901234567890123456789")] // 29 chars
    [InlineData("1234567890123456789012345678901")] // 31 chars
    [InlineData("abc123456789012345678901234567")] // lowercase
    [InlineData("12345678901234567890123456789!")] // special char
    public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => _ = new IdentificacaoObra(value!);

        // Assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        const string value = "ABCDEFGHIJ12345678901234567890";

        // Act
        var obra = IdentificacaoObra.FromString(value);

        // Assert
        Assert.NotNull(obra);
        Assert.Equal(value, obra.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        const string value = "ZYXWVUTSRQPONMLKJIHGFEDCBA1234";

        // Act
        var obra = (IdentificacaoObra)value;

        // Assert
        Assert.NotNull(obra);
        Assert.Equal(value, obra.ToString());
    }

    [Fact]
    public void XmlDeserialization_InvalidValue_ThrowsSerializationException()
    {
        // Arrange — letras minúsculas violam o padrão [0-9A-Z]{30}
        const string xml = """<?xml version="1.0" encoding="utf-16"?><IdentificacaoObra xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">abc123456789012345678901234567</IdentificacaoObra>""";

        // Act
        Action act = () => DesserializarDeXml(xml);

        // Assert
        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.IsType<SerializationException>(ex.InnerException);
    }

    [Fact]
    public void XmlDeserialization_ValidValue_DeserializesCorrectly()
    {
        // Arrange
        const string value = "123456789012345678901234567890";
        var xml = SerializarParaXml(new IdentificacaoObra(value));

        // Act
        var desserializado = DesserializarDeXml(xml);

        // Assert
        Assert.Equal(value, desserializado!.ToString());
    }

    private static string SerializarParaXml(IdentificacaoObra obra)
    {
        var serializer = new XmlSerializer(typeof(IdentificacaoObra));
        using var sw = new StringWriter();
        serializer.Serialize(sw, obra);
        return sw.ToString();
    }

    private static IdentificacaoObra? DesserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(IdentificacaoObra));
        using var sr = new StringReader(xml);
        return (IdentificacaoObra?)serializer.Deserialize(sr);
    }
}