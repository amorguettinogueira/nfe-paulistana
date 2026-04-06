using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;

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
}