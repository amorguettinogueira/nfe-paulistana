using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using System.Globalization;
using System.Runtime.Serialization;

namespace Nfe.Paulistana.Tests.V1.Models.DataTypes;

public class CnpjTests
{
    private static string FormatCnpj(long value)
    {
        var s = value.ToString("D14", CultureInfo.InvariantCulture);
        //01234567890123
        //01.234.567/8901-23
        return $"{s.Substring(0, 2)}.{s.Substring(2, 3)}.{s.Substring(5, 3)}/{s.Substring(8, 4)}-{s.Substring(12, 2)}";
    }

    // ============================================
    // Construction Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_FromLong_FormatsValue(long value)
    {
        var cnpj = new Cnpj(value);

        Assert.Equal(value.ToString("D14", CultureInfo.InvariantCulture), cnpj.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_FromString_WithFormatting(long value)
    {
        var formatted = FormatCnpj(value);
        var cnpj = new Cnpj(formatted);

        Assert.Equal(value.ToString("D14", CultureInfo.InvariantCulture), cnpj.ToString());
    }

    [Fact]
    public void Cnpj_InvalidNumberString_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj("XYZ/ABC"));
        Assert.Contains("CNPJ não é um número válido", ex.Message);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_InvalidCheckDigits_Throws(long valid)
    {
        var invalid = valid + 1;
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(invalid));
        Assert.Contains("CNPJ não tem dígitos", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_MinValueViolation_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(999999));
        Assert.Contains("pelo menos", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_MaxValueViolation_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(100_000_000_000_000));
        Assert.Contains("máximo", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    // ============================================
    // Operator Tests
    // ============================================

    [Fact]
    public void ExplicitOperators_CreateInstances()
    {
        long cnpjNumber = Helpers.TestConstants.ValidCnpj;
        Cnpj cnpjFromLong = (Cnpj)cnpjNumber;
        Cnpj cnpjFromString = (Cnpj)FormatCnpj(cnpjNumber);

        Assert.Equal(cnpjFromLong.ToString(), cnpjFromString.ToString());
    }

    // ============================================
    // Immutability Tests
    // ============================================

    [Fact]
    public void Cnpj_IsImmutable_ValuePropertyHasNoPublicSetter()
    {
        // Arrange
        var cnpjType = typeof(Cnpj);
        var valueProperty = cnpjType.GetProperty("Value",
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
            "Cnpj should be immutable: Value property either doesn't exist publicly " +
            "or has no public setter (init-only property)");
    }

    // ============================================
    // All Same Digits Rejection Tests
    // ============================================

    [Theory]
    [InlineData(11111111111111)]
    [InlineData(22222222222222)]
    [InlineData(33333333333333)]
    [InlineData(44444444444444)]
    [InlineData(55555555555555)]
    [InlineData(66666666666666)]
    [InlineData(77777777777777)]
    [InlineData(88888888888888)]
    [InlineData(99999999999999)]
    public void Cnpj_AllSameDigits_Throws(long invalidCnpj)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj(invalidCnpj));
        Assert.Contains("CNPJ não tem dígitos", ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    // ============================================
    // XML Serialization Tests
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_XmlSerialization_RoundTrip(long value)
    {
        // Arrange
        var cnpj = new Cnpj(value);

        // Act
        var xml = XmlTestHelper.SerializarParaXml(cnpj);
        var deserialized = XmlTestHelper.DesserializarDeXml<Cnpj>(xml);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(cnpj.ToString(), deserialized!.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_XmlSerialization_ValidValue_Deserializes(long value)
    {
        // Arrange
        var cnpjValue = value.ToString("D14", CultureInfo.InvariantCulture);
        var cnpjXml = $@"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">{cnpjValue}</Cnpj>";

        // Act
        var deserialized = XmlTestHelper.DesserializarDeXml<Cnpj>(cnpjXml);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(cnpjValue, deserialized!.ToString());
    }

    // ============================================
    // XML Deserialization Exception Tests
    // ============================================

    [Fact]
    public void Cnpj_XmlDeserialization_InvalidCheckDigits_ThrowsSerializationException()
    {
        // Arrange - Use an invalid CNPJ value
        var invalidCnpjXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">00011111111111</Cnpj>"; // Invalid checksum

        // Act & Assert
        // Note: XmlSerializer wraps exceptions thrown during IXmlSerializable.ReadXml() in InvalidOperationException.
        // We verify the underlying SerializationException to properly test our validation logic without coupling
        // the test to XmlSerializer's implementation details.
        var ex = Assert.Throws<InvalidOperationException>(() =>
            XmlTestHelper.DesserializarDeXml<Cnpj>(invalidCnpjXml));

        // Verify the actual validation exception is in the inner exception chain
        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_XmlDeserialization_AllSameDigits_ThrowsSerializationException()
    {
        // Arrange - All same digits
        var invalidCnpjXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">11111111111111</Cnpj>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            XmlTestHelper.DesserializarDeXml<Cnpj>(invalidCnpjXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_XmlDeserialization_MinValueViolation_ThrowsSerializationException()
    {
        // Arrange - Below minimum
        var invalidCnpjXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">00000000000000</Cnpj>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            XmlTestHelper.DesserializarDeXml<Cnpj>(invalidCnpjXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_XmlDeserialization_MaxValueViolation_ThrowsSerializationException()
    {
        // Arrange - Above maximum
        var invalidCnpjXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">999999999999999999</Cnpj>"; // Way too large

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            XmlTestHelper.DesserializarDeXml<Cnpj>(invalidCnpjXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_XmlDeserialization_InvalidFormat_ThrowsSerializationException()
    {
        // Arrange - Non-numeric value
        var invalidCnpjXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">ABC/XYZ-XXXX</Cnpj>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            XmlTestHelper.DesserializarDeXml<Cnpj>(invalidCnpjXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Cnpj_XmlDeserialization_EmptyValue_ThrowsSerializationException()
    {
        // Arrange - Empty string
        var invalidCnpjXml = @"<?xml version=""1.0"" encoding=""utf-16""?><Cnpj xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""></Cnpj>";

        // Act & Assert
        // XmlSerializer wraps exceptions in InvalidOperationException; verify the inner SerializationException
        var ex = Assert.Throws<InvalidOperationException>(() =>
            XmlTestHelper.DesserializarDeXml<Cnpj>(invalidCnpjXml));

        var serializationEx = Assert.IsType<SerializationException>(ex.InnerException);
        Assert.Contains("desserializado", serializationEx.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void Cnpj_IsSealed() =>
        Assert.True(typeof(Cnpj).IsSealed);

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_Equals_SameValue_ReturnsTrue(long value) =>
        Assert.Equal(new Cnpj(value), new Cnpj(value));

    [Fact]
    public void Cnpj_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Cnpj(84067820000190), new Cnpj(33579346000145));

    [Fact]
    public void Cnpj_Equals_NullObject_ReturnsFalse() =>
        Assert.False(new Cnpj(84067820000190).Equals(null));

    [Theory]
    [ClassData(typeof(ValidCnpjNumbers))]
    public void Cnpj_GetHashCode_SameValue_ReturnsSameHash(long value) =>
        Assert.Equal(new Cnpj(value).GetHashCode(), new Cnpj(value).GetHashCode());

    [Fact]
    public void Cnpj_GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new Cnpj(84067820000190).GetHashCode(), new Cnpj(33579346000145).GetHashCode());

    // ============================================
    // FromString / FromInt64
    // ============================================

    [Fact]
    public void FromString_ValorValido_CriaInstanciaCorreta() =>
        Assert.Equal("84067820000190", Cnpj.FromString("84.067.820/0001-90").ToString());

    [Fact]
    public void FromInt64_ValorValido_CriaInstanciaCorreta() =>
        Assert.Equal("84067820000190", Cnpj.FromInt64(84067820000190L).ToString());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_StringNula_RetornaNull() =>
        Assert.Null(Cnpj.ParseIfPresent((string?)null));

    [Fact]
    public void ParseIfPresent_StringVazia_RetornaNull() =>
        Assert.Null(Cnpj.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_StringValida_RetornaInstancia() =>
        Assert.Equal("84067820000190", Cnpj.ParseIfPresent("84067820000190")!.ToString());

    // ============================================
    // ParseIfPresent(long?)
    // ============================================

    [Fact]
    public void ParseIfPresent_LongNulo_RetornaNull() =>
        Assert.Null(Cnpj.ParseIfPresent((long?)null));

    [Fact]
    public void ParseIfPresent_LongValido_RetornaInstancia() =>
        Assert.Equal("84067820000190", Cnpj.ParseIfPresent((long?)84067820000190L)!.ToString());
}