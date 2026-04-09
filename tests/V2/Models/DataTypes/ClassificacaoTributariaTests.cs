using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="ClassificacaoTributaria"/>.
/// </summary>
public sealed class ClassificacaoTributariaTests
{
    [Theory]
    [InlineData("123456")]
    [InlineData("000001")]
    [InlineData("999999")]
    public void Constructor_ValidValue_ShouldSetValue(string value)
    {
        // Act
        var classificacao = new ClassificacaoTributaria(value);

        // Assert
        Assert.NotNull(classificacao);
        Assert.Equal(value, classificacao.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345")] // menos de 6
    [InlineData("1234567")] // mais de 6
    [InlineData("12A456")] // caractere inválido
    public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => _ = new ClassificacaoTributaria(value!);

        // Assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "123456";

        // Act
        var classificacao = ClassificacaoTributaria.FromString(value);

        // Assert
        Assert.NotNull(classificacao);
        Assert.Equal(value, classificacao.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "123456";

        // Act
        var classificacao = (ClassificacaoTributaria)value;

        // Assert
        Assert.NotNull(classificacao);
        Assert.Equal(value, classificacao.ToString());
    }

    [Fact]
    public void OnXmlDeserialized_InvalidValue_ShouldThrowSerializationException()
    {
        // Arrange
        var classificacao = (ClassificacaoTributaria)Activator.CreateInstance(typeof(ClassificacaoTributaria), true)!;
        var valueField = typeof(ClassificacaoTributaria).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance)!;
        valueField.SetValue(classificacao, "12A456");

        // Act
        Action act = () => typeof(ClassificacaoTributaria).GetMethod("OnXmlDeserialized", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(classificacao, null);

        // Assert
        var ex = Assert.Throws<TargetInvocationException>(act);
        Assert.IsType<SerializationException>(ex.InnerException);
    }

    // ============================================
    // Construtor padrão / sealed
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new ClassificacaoTributaria().ToString());

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(ClassificacaoTributaria).IsSealed);

    // ============================================
    // Equals / GetHashCode
    // ============================================

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new ClassificacaoTributaria("123456"), new ClassificacaoTributaria("123456"));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new ClassificacaoTributaria("123456"), new ClassificacaoTributaria("999999"));

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new ClassificacaoTributaria("123456").Equals(null));

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new ClassificacaoTributaria("123456").GetHashCode(), new ClassificacaoTributaria("123456").GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new ClassificacaoTributaria("123456").GetHashCode(), new ClassificacaoTributaria("999999").GetHashCode());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(ClassificacaoTributaria.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(ClassificacaoTributaria.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_WithValidString_ReturnsInstance() =>
        Assert.Equal(new ClassificacaoTributaria("123456"), ClassificacaoTributaria.ParseIfPresent("123456"));

    [Fact]
    public void ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => ClassificacaoTributaria.ParseIfPresent("12A456"));

    // ============================================
    // XML round-trip
    // ============================================

    [Fact]
    public void XmlRoundTrip_ValidValue_PreservesValue()
    {
        var original = new ClassificacaoTributaria("123456");
        var serializer = new XmlSerializer(typeof(ClassificacaoTributaria));

        using var sw = new StringWriter();
        serializer.Serialize(sw, original);
        using var sr = new StringReader(sw.ToString());
        var deserialized = (ClassificacaoTributaria?)serializer.Deserialize(sr);

        Assert.Equal("123456", deserialized?.ToString());
    }

    [Fact]
    public void XmlDeserialization_InvalidValue_ThrowsSerializationException()
    {
        const string xml = """<?xml version="1.0" encoding="utf-16"?><ClassificacaoTributaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">12A456</ClassificacaoTributaria>""";
        var serializer = new XmlSerializer(typeof(ClassificacaoTributaria));

        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.IsType<SerializationException>(ex.InnerException);
    }
}