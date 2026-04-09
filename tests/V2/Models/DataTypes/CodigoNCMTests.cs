using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoNCM"/>.
/// </summary>
public sealed class CodigoNCMTests
{
    // =============================
    // CodigoNCM
    // =============================

    [Fact]
    public void CodigoNCM_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoNCM().ToString());

    [Theory]
    [InlineData("12345678")]
    [InlineData("00000001")]
    [InlineData("99999999")]
    public void CodigoNCM_ValidValue_ShouldSetValue(string value)
    {
        var ncm = new CodigoNCM(value);
        Assert.NotNull(ncm);
        Assert.Equal(value, ncm.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567")] // menos de 8
    [InlineData("123456789")] // mais de 8
    [InlineData("12A45678")] // caractere inválido
    public void CodigoNCM_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => new CodigoNCM(value!);
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void CodigoNCM_FromString_ValidValue_ShouldReturnInstance()
    {
        const string value = "12345678";
        var ncm = CodigoNCM.FromString(value);
        Assert.NotNull(ncm);
        Assert.Equal(value, ncm.ToString());
    }

    [Fact]
    public void CodigoNCM_ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        const string value = "12345678";
        var ncm = (CodigoNCM)value;
        Assert.NotNull(ncm);
        Assert.Equal(value, ncm.ToString());
    }

    [Fact]
    public void CodigoNCM_OnXmlDeserialized_InvalidValue_ShouldThrowSerializationException()
    {
        var ncm = (CodigoNCM)Activator.CreateInstance(typeof(CodigoNCM), true)!;
        typeof(CodigoNCM).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(ncm, "12A45678");
        Action act = () => typeof(CodigoNCM).GetMethod("OnXmlDeserialized", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(ncm, null);
        var ex = Assert.Throws<TargetInvocationException>(act);
        Assert.IsType<SerializationException>(ex.InnerException);
    }

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void CodigoNCM_IsSealed() =>
        Assert.True(typeof(CodigoNCM).IsSealed);

    [Fact]
    public void CodigoNCM_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new CodigoNCM("12345678"), new CodigoNCM("12345678"));

    [Fact]
    public void CodigoNCM_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new CodigoNCM("12345678"), new CodigoNCM("99999999"));

    [Fact]
    public void CodigoNCM_Equals_NullObject_ReturnsFalse() =>
        Assert.False(new CodigoNCM("12345678").Equals(null));

    [Fact]
    public void CodigoNCM_Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        var ncm = new CodigoNCM("12345678");
        var cadastro = new CadastroImovel("12345678");

        Assert.False(ncm.Equals(cadastro));
    }

    [Fact]
    public void CodigoNCM_GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new CodigoNCM("12345678").GetHashCode(), new CodigoNCM("12345678").GetHashCode());

    [Fact]
    public void CodigoNCM_GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new CodigoNCM("12345678").GetHashCode(), new CodigoNCM("99999999").GetHashCode());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void CodigoNCM_ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(CodigoNCM.ParseIfPresent(null));

    [Fact]
    public void CodigoNCM_ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(CodigoNCM.ParseIfPresent("   "));

    [Fact]
    public void CodigoNCM_ParseIfPresent_WithValidString_ReturnsInstance() =>
        Assert.Equal(new CodigoNCM("12345678"), CodigoNCM.ParseIfPresent("12345678"));

    [Fact]
    public void CodigoNCM_ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoNCM.ParseIfPresent("12A45678"));

    // ============================================
    // XML round-trip
    // ============================================

    [Fact]
    public void CodigoNCM_XmlRoundTrip_ValidValue_PreservesValue()
    {
        var original = new CodigoNCM("12345678");
        var serializer = new XmlSerializer(typeof(CodigoNCM));

        using var sw = new StringWriter();
        serializer.Serialize(sw, original);
        using var sr = new StringReader(sw.ToString());
        var deserialized = (CodigoNCM?)serializer.Deserialize(sr);

        Assert.Equal("12345678", deserialized?.ToString());
    }

    [Fact]
    public void CodigoNCM_XmlDeserialization_InvalidValue_ThrowsSerializationException()
    {
        const string xml = """<?xml version="1.0" encoding="utf-16"?><CodigoNCM xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">12A45678</CodigoNCM>""";
        var serializer = new XmlSerializer(typeof(CodigoNCM));

        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.IsType<SerializationException>(ex.InnerException);
    }
}