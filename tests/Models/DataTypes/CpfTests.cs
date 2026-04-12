using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class CpfTests
{
    private static string FormatCpf(long value)
    {
        var s = value.ToString("D11", CultureInfo.InvariantCulture);
        return $"{s.Substring(0, 3)}.{s.Substring(3, 3)}.{s.Substring(6, 3)}-{s.Substring(9, 2)}";
    }

    private static string SerializeCpfToXml(Cpf cpf)
    {
        var serializer = new XmlSerializer(typeof(Cpf));
        using (var stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, cpf);
            return stringWriter.ToString();
        }
    }

    private static Cpf? DeserializeCpfFromXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(Cpf));
        using (var stringReader = new StringReader(xml))
        {
            return (Cpf?)serializer.Deserialize(stringReader);
        }
    }

    // ============================================
    // Construction Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_FromLong_FormatsValue(long value)
    {
        var cpf = new Cpf(value);

        Assert.Equal(value.ToString("D11", CultureInfo.InvariantCulture), cpf.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_FromString_WithFormatting(long value)
    {
        var formatted = FormatCpf(value);
        var cpf = new Cpf(formatted);

        Assert.Equal(value.ToString("D11", CultureInfo.InvariantCulture), cpf.ToString());
    }

    [Fact]
    public void Cpf_InvalidNumberString_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cpf("ABC-XYZ"));
        Assert.Contains("CPF não é um número válido", ex.Message);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_InvalidCheckDigits_Throws(long valid)
    {
        var invalid = valid + 1; // tweak check digits
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalid));
        Assert.Contains("CPF não tem dígitos", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_MinValueViolation_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(99));
        Assert.Contains("pelo menos", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_MaxValueViolation_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(100_000_000_000));
        Assert.Contains("no máximo", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    // ============================================
    // Operator Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void ExplicitOperators_CreateInstances(long cpfNumber)
    {
        Cpf cpfFromLong = (Cpf)cpfNumber;
        Cpf cpfFromString = (Cpf)FormatCpf(cpfNumber);

        Assert.Equal(cpfFromLong.ToString(), cpfFromString.ToString());
    }

    // ============================================
    // Immutability Tests
    // ============================================

    [Fact]
    public void Cpf_IsImmutable_ValuePropertyHasNoPublicSetter()
    {
        // Arrange
        var cpfType = typeof(Cpf);
        var valueProperty = cpfType.GetProperty("Value",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.IgnoreCase);

        // Act
        var publicSetter = valueProperty?.GetSetMethod(nonPublic: false);

        // Assert - Immutable if:
        // 1. Property doesn't exist as public, OR
        // 2. Property exists but has no public setter (init-only)
        Assert.True(
            valueProperty == null || publicSetter == null,
            "Cpf should be immutable: Value property either doesn't exist publicly " +
            "or has no public setter (init-only property)");
    }

    // ============================================
    // All Same Digits Rejection Tests
    // ============================================

    [Theory]
    [InlineData(11111111111)]
    [InlineData(22222222222)]
    [InlineData(33333333333)]
    [InlineData(44444444444)]
    [InlineData(55555555555)]
    [InlineData(66666666666)]
    [InlineData(77777777777)]
    [InlineData(88888888888)]
    [InlineData(99999999999)]
    public void Cpf_AllSameDigits_Throws(long invalidCpf)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cpf(invalidCpf));
        Assert.Contains("CPF não tem dígitos", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    // ============================================
    // XML Serialization Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_XmlSerialization_RoundTrip(long value)
    {
        // Arrange
        var cpf = new Cpf(value);

        // Act
        var xml = SerializeCpfToXml(cpf);
        var deserialized = DeserializeCpfFromXml(xml);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(cpf.ToString(), deserialized!.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_XmlSerialization_ValidValue_Deserializes(long value)
    {
        // Arrange
        var cpfValue = value.ToString("D11", CultureInfo.InvariantCulture);
        var cpfXml = $@"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">{cpfValue}</Cpf>";

        // Act
        var deserialized = DeserializeCpfFromXml(cpfXml);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(cpfValue, deserialized!.ToString());
    }

    // ============================================
    // XML Deserialization Exception Tests
    // ============================================

    [Fact]
    public void Cpf_XmlDeserialization_InvalidCheckDigits_ThrowsSerializationException()
    {
        // Arrange - Invalid checksum
        var invalidCpfXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">00011111111</Cpf>";

        // Act & Assert
        // Note: XmlSerializer wraps exceptions thrown during IXmlSerializable.ReadXml() in InvalidOperationException.
        // We verify the underlying SerializationException to properly test our validation logic without coupling
        // the test to XmlSerializer's implementation details.
        var ex = Assert.Throws<InvalidOperationException>(() =>
            DeserializeCpfFromXml(invalidCpfXml));

        // Verify the actual validation exception is in the inner exception chain
        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_XmlDeserialization_AllSameDigits_ThrowsSerializationException()
    {
        // Arrange - All same digits
        var invalidCpfXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">11111111111</Cpf>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            DeserializeCpfFromXml(invalidCpfXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_XmlDeserialization_MinValueViolation_ThrowsSerializationException()
    {
        // Arrange - Below minimum
        var invalidCpfXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">00000000000</Cpf>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            DeserializeCpfFromXml(invalidCpfXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_XmlDeserialization_MaxValueViolation_ThrowsSerializationException()
    {
        // Arrange - Above maximum
        var invalidCpfXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">99999999999999999</Cpf>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            DeserializeCpfFromXml(invalidCpfXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_XmlDeserialization_InvalidFormat_ThrowsSerializationException()
    {
        // Arrange - Non-numeric value
        var invalidCpfXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">ABC-123-XXXX</Cpf>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            DeserializeCpfFromXml(invalidCpfXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cpf_XmlDeserialization_EmptyValue_ThrowsSerializationException()
    {
        // Arrange - Empty string
        var invalidCpfXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cpf xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""></Cpf>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            DeserializeCpfFromXml(invalidCpfXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void Cpf_IsSealed() =>
        Assert.True(typeof(Cpf).IsSealed);

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_Equals_SameValue_ReturnsTrue(long value) =>
        Assert.Equal(new Cpf(value), new Cpf(value));

    [Fact]
    public void Cpf_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Cpf(63596780047), new Cpf(86290818210));

    [Fact]
    public void Cpf_Equals_NullObject_ReturnsFalse() =>
        Assert.False(new Cpf(63596780047).Equals(null));

    [Theory]
    [ClassData(typeof(ValidCpfNumbers))]
    public void Cpf_GetHashCode_SameValue_ReturnsSameHash(long value) =>
        Assert.Equal(new Cpf(value).GetHashCode(), new Cpf(value).GetHashCode());

    [Fact]
    public void Cpf_GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new Cpf(63596780047).GetHashCode(), new Cpf(86290818210).GetHashCode());

    // ============================================
    // FromString / FromInt64
    // ============================================

    [Fact]
    public void FromString_ValorValido_CriaInstanciaCorreta() =>
        Assert.Equal("46381819618", Cpf.FromString("463.818.196-18").ToString());

    [Fact]
    public void FromInt64_ValorValido_CriaInstanciaCorreta() =>
        Assert.Equal("46381819618", Cpf.FromInt64(46381819618L).ToString());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_StringNula_RetornaNull() =>
        Assert.Null(Cpf.ParseIfPresent((string?)null));

    [Fact]
    public void ParseIfPresent_StringVazia_RetornaNull() =>
        Assert.Null(Cpf.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_StringValida_RetornaInstancia() =>
        Assert.Equal("46381819618", Cpf.ParseIfPresent("46381819618")!.ToString());

    // ============================================
    // ParseIfPresent(long?)
    // ============================================

    [Fact]
    public void ParseIfPresent_LongNulo_RetornaNull() =>
        Assert.Null(Cpf.ParseIfPresent((long?)null));

    [Fact]
    public void ParseIfPresent_LongValido_RetornaInstancia() =>
        Assert.Equal("46381819618", Cpf.ParseIfPresent((long?)46381819618L)!.ToString());
}