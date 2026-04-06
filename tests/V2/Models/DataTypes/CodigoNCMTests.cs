using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;

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
}