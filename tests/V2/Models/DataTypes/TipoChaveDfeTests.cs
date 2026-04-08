using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="TipoChaveDfe"/>.
/// </summary>
public sealed class TipoChaveDfeTests
{
    // =============================
    // TipoChaveDfe
    // =============================

    [Fact]
    public void TipoChaveDfe_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new TipoChaveDfe().ToString());

    [Theory]
    [InlineData("Outro documento")]
    [InlineData("1234567890")] // 10 chars
    [InlineData("A")] // 1 char
    [InlineData(" 	Descrição com espaços ")]
    [InlineData("Descrição com & < > \" '")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345")] // 255 chars
    public void TipoChaveDfe_ValidValue_ShouldSetValue(string value)
    {
        var tipo = new TipoChaveDfe(value);
        Assert.NotNull(tipo);
        Assert.NotNull(tipo.ToString());
        Assert.True(tipo.ToString()!.Length <= 255);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456")] // 256 chars
    public void TipoChaveDfe_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => new TipoChaveDfe(value!);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void TipoChaveDfe_FromString_ValidValue_ShouldReturnInstance()
    {
        const string value = "Tipo válido";
        var tipo = TipoChaveDfe.FromString(value);
        Assert.NotNull(tipo);
        Assert.Equal("Tipo válido", tipo.ToString());
    }

    [Fact]
    public void TipoChaveDfe_ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        const string value = "Tipo válido";
        var tipo = (TipoChaveDfe)value;
        Assert.NotNull(tipo);
        Assert.Equal("Tipo válido", tipo.ToString());
    }

    [Fact]
    public void TipoChaveDfe_OnXmlDeserialized_InvalidValue_ShouldThrowSerializationException()
    {
        var tipo = (TipoChaveDfe)Activator.CreateInstance(typeof(TipoChaveDfe), true)!;
        typeof(TipoChaveDfe).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(tipo, new string('B', 256));
        Action act = () => typeof(TipoChaveDfe).GetMethod("OnXmlDeserialized", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(tipo, null);
        Assert.Throws<TargetInvocationException>(act);
    }

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(TipoChaveDfe.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(TipoChaveDfe.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(TipoChaveDfe.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("Tipo válido", TipoChaveDfe.ParseIfPresent("Tipo válido")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => TipoChaveDfe.ParseIfPresent(new string('A', 256)));
}