using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="NumeroDescricaoDocumento"/>.
/// </summary>
public sealed class NumeroDescricaoDocumentoTests
{
    // =============================
    // NumeroDescricaoDocumento
    // =============================

    [Fact]
    public void NumeroDescricaoDocumento_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new NumeroDescricaoDocumento().ToString());

    [Theory]
    [InlineData("Documento 1")]
    [InlineData("1234567890")] // 10 chars
    [InlineData("A")] // 1 char
    [InlineData(" 	Documento com espaços ")]
    [InlineData("Descrição com & < > \" '")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345")] // 255 chars
    public void NumeroDescricaoDocumento_ValidValue_ShouldSetValue(string value)
    {
        var doc = new NumeroDescricaoDocumento(value);
        Assert.NotNull(doc);
        Assert.NotNull(doc.ToString());
        Assert.True(doc.ToString()!.Length <= 255);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456")] // 256 chars
    public void NumeroDescricaoDocumento_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => new NumeroDescricaoDocumento(value!);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void NumeroDescricaoDocumento_FromString_ValidValue_ShouldReturnInstance()
    {
        const string value = "Documento válido";
        var doc = NumeroDescricaoDocumento.FromString(value);
        Assert.NotNull(doc);
        Assert.Equal("Documento válido", doc.ToString());
    }

    [Fact]
    public void NumeroDescricaoDocumento_ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        const string value = "Documento válido";
        var doc = (NumeroDescricaoDocumento)value;
        Assert.NotNull(doc);
        Assert.Equal("Documento válido", doc.ToString());
    }

    [Fact]
    public void NumeroDescricaoDocumento_OnXmlDeserialized_InvalidValue_ShouldThrowSerializationException()
    {
        var doc = (NumeroDescricaoDocumento)Activator.CreateInstance(typeof(NumeroDescricaoDocumento), true)!;
        typeof(NumeroDescricaoDocumento).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(doc, new string('A', 256));
        Action act = () => typeof(NumeroDescricaoDocumento).GetMethod("OnXmlDeserialized", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(doc, null);
        Assert.Throws<TargetInvocationException>(act);
    }
}