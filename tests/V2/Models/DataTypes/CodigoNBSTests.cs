using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoNBS"/>.
/// </summary>
public sealed class CodigoNBSTests
{
    // =============================
    // CodigoNBS
    // =============================

    [Fact]
    public void CodigoNBS_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoNBS().ToString());

    [Theory]
    [InlineData("123456789")]
    [InlineData("000000001")]
    [InlineData("999999999")]
    public void CodigoNBS_ValidValue_ShouldSetValue(string value)
    {
        var nbs = new CodigoNBS(value);
        Assert.NotNull(nbs);
        Assert.Equal(value, nbs.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678")] // menos de 9
    [InlineData("1234567890")] // mais de 9
    [InlineData("12A456789")] // caractere inválido
    public void CodigoNBS_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => new CodigoNBS(value!);
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void CodigoNBS_FromString_ValidValue_ShouldReturnInstance()
    {
        const string value = "123456789";
        var nbs = CodigoNBS.FromString(value);
        Assert.NotNull(nbs);
        Assert.Equal(value, nbs.ToString());
    }

    [Fact]
    public void CodigoNBS_ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        const string value = "123456789";
        var nbs = (CodigoNBS)value;
        Assert.NotNull(nbs);
        Assert.Equal(value, nbs.ToString());
    }

    [Fact]
    public void CodigoNBS_OnXmlDeserialized_InvalidValue_ShouldThrowSerializationException()
    {
        var nbs = (CodigoNBS)Activator.CreateInstance(typeof(CodigoNBS), true)!;
        typeof(CodigoNBS).GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(nbs, "12A456789");
        Action act = () => typeof(CodigoNBS).GetMethod("OnXmlDeserialized", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(nbs, null);
        var ex = Assert.Throws<TargetInvocationException>(act);
        Assert.IsType<SerializationException>(ex.InnerException);
    }
}