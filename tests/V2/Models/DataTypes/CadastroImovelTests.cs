using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CadastroImovel"/>.
/// </summary>
public sealed class CadastroImovelTests
{
    [Theory]
    [InlineData("ABCDEFGH")]
    [InlineData("12345678")]
    [InlineData("A1B2C3D4")]
    public void Constructor_ValidValue_ShouldSetValue(string value)
    {
        // Act
        var cadastro = new CadastroImovel(value);

        // Assert
        Assert.NotNull(cadastro);
        Assert.Equal(value, cadastro.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abc12345")] // minúsculas
    [InlineData("1234567")]  // menos de 8
    [InlineData("123456789")] // mais de 8
    [InlineData("1234-678")] // caractere inválido
    public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => _ = new CadastroImovel(value!);

        // Assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "ABCDEFGH";

        // Act
        var cadastro = CadastroImovel.FromString(value);

        // Assert
        Assert.NotNull(cadastro);
        Assert.Equal(value, cadastro.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "ABCDEFGH";

        // Act
        var cadastro = (CadastroImovel)value;

        // Assert
        Assert.NotNull(cadastro);
        Assert.Equal(value, cadastro.ToString());
    }

    [Fact]
    public void OnXmlDeserialized_InvalidValue_ShouldThrowSerializationException()
    {
        // Arrange
        var cadastro = (CadastroImovel)Activator.CreateInstance(typeof(CadastroImovel), true)!;
        var valueField = typeof(CadastroImovel).GetProperty("Value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        valueField.SetValue(cadastro, "1234-678");

        // Act
        Action act = () => typeof(CadastroImovel).GetMethod("OnXmlDeserialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(cadastro, null);

        // Assert
        var ex = Assert.Throws<TargetInvocationException>(act);
        Assert.IsType<SerializationException>(ex.InnerException);
    }

    // ============================================
    // Construtor padrão / sealed
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CadastroImovel().ToString());

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CadastroImovel).IsSealed);

    // ============================================
    // Equals / GetHashCode
    // ============================================

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new CadastroImovel("ABCDEFGH"), new CadastroImovel("ABCDEFGH"));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new CadastroImovel("ABCDEFGH"), new CadastroImovel("12345678"));

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new CadastroImovel("ABCDEFGH").Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        var cadastro = new CadastroImovel("12345678");
        var ncm = new CodigoNCM("12345678");

        Assert.False(cadastro.Equals(ncm));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new CadastroImovel("ABCDEFGH").GetHashCode(), new CadastroImovel("ABCDEFGH").GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new CadastroImovel("ABCDEFGH").GetHashCode(), new CadastroImovel("12345678").GetHashCode());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(CadastroImovel.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(CadastroImovel.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_WithValidString_ReturnsCadastroImovel() =>
        Assert.Equal(new CadastroImovel("ABCDEFGH"), CadastroImovel.ParseIfPresent("ABCDEFGH"));

    [Fact]
    public void ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CadastroImovel.ParseIfPresent("1234-678"));

    // ============================================
    // XML round-trip
    // ============================================

    [Fact]
    public void XmlRoundTrip_ValidValue_PreservesValue()
    {
        var original = new CadastroImovel("A1B2C3D4");
        var serializer = new XmlSerializer(typeof(CadastroImovel));

        using var sw = new StringWriter();
        serializer.Serialize(sw, original);
        using var sr = new StringReader(sw.ToString());
        var deserialized = (CadastroImovel?)serializer.Deserialize(sr);

        Assert.Equal("A1B2C3D4", deserialized?.ToString());
    }

    [Fact]
    public void XmlDeserialization_InvalidValue_ThrowsSerializationException()
    {
        const string xml = """<?xml version="1.0" encoding="utf-16"?><CadastroImovel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">1234-678</CadastroImovel>""";
        var serializer = new XmlSerializer(typeof(CadastroImovel));

        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.IsType<SerializationException>(ex.InnerException);
    }
}