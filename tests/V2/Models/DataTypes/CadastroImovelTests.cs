using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;
using System.Runtime.Serialization;

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
}