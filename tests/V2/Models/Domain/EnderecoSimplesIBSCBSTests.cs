using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class EnderecoSimplesIBSCBSTests
{
    [Fact]
    public void EnderecoSimplesIBSCBS_DefaultConstructor_AllPropertiesNull()
    {
        // Arrange & Act
        var end = new EnderecoSimplesIBSCBS();

        // Assert
        Assert.Null(end.Cep);
        Assert.Null(end.EnderecoExterior);
        Assert.Null(end.Logradouro);
        Assert.Null(end.Numero);
        Assert.Null(end.Complemento);
        Assert.Null(end.Bairro);
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Constructor_WithCep_SetsProperties()
    {
        // Arrange
        var logradouro = new Logradouro("Rua A");
        var numero = new NumeroEndereco("10");
        var bairro = new Bairro("Centro");
        var cep = new Cep(12345678);
        var complemento = new Complemento("Apto 1");

        // Act
        var end = new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cep, null, complemento);

        // Assert
        Assert.Equal(cep, end.Cep);
        Assert.Null(end.EnderecoExterior);
        Assert.Equal(logradouro, end.Logradouro);
        Assert.Equal(numero, end.Numero);
        Assert.Equal(complemento, end.Complemento);
        Assert.Equal(bairro, end.Bairro);
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Constructor_WithEnderecoExterior_SetsProperties()
    {
        // Arrange
        var logradouro = new Logradouro("Rua B");
        var numero = new NumeroEndereco("20");
        var bairro = new Bairro("Bairro B");
        var complemento = new Complemento("Casa");
        var enderecoExterior = new EnderecoExterior(
            new CodigoPaisISO("US"),
            new CodigoEndPostal("12345"),
            new NomeCidade("New York"),
            new EstadoProvinciaRegiao("NY")
        );

        // Act
        var end = new EnderecoSimplesIBSCBS(logradouro, numero, bairro, null, enderecoExterior, complemento);

        // Assert
        Assert.Null(end.Cep);
        Assert.Equal(enderecoExterior, end.EnderecoExterior);
        Assert.Equal(logradouro, end.Logradouro);
        Assert.Equal(numero, end.Numero);
        Assert.Equal(complemento, end.Complemento);
        Assert.Equal(bairro, end.Bairro);
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Constructor_NullLogradouro_ThrowsArgumentNullException()
    {
        // Arrange
        Logradouro? logradouro = null;
        var numero = new NumeroEndereco("1");
        var bairro = new Bairro("Centro");
        var cep = new Cep(12345678);

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cep, null, null));
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Constructor_NullNumero_ThrowsArgumentNullException()
    {
        // Arrange
        var logradouro = new Logradouro("Rua");
        NumeroEndereco? numero = null;
        var bairro = new Bairro("Centro");
        var cep = new Cep(12345678);

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cep, null, null));
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Constructor_NullBairro_ThrowsArgumentNullException()
    {
        // Arrange
        var logradouro = new Logradouro("Rua");
        var numero = new NumeroEndereco("1");
        Bairro? bairro = null;
        var cep = new Cep(12345678);

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cep, null, null));
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Constructor_NoCepNorEnderecoExterior_ThrowsArgumentException()
    {
        // Arrange
        var logradouro = new Logradouro("Rua");
        var numero = new NumeroEndereco("1");
        var bairro = new Bairro("Centro");

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => new EnderecoSimplesIBSCBS(logradouro, numero, bairro, null, null, null));
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Setters_PropertiesSetCorrectly()
    {
        // Arrange
        var end = new EnderecoSimplesIBSCBS();
        var logradouro = new Logradouro("Rua Nova");
        var numero = new NumeroEndereco("99");
        var bairro = new Bairro("Bairro Novo");
        var cep = new Cep(87654321);
        var complemento = new Complemento("Fundos");

        // Act
        end.Logradouro = logradouro;
        end.Numero = numero;
        end.Bairro = bairro;
        end.Cep = cep;
        end.Complemento = complemento;

        // Assert
        Assert.Equal(logradouro, end.Logradouro);
        Assert.Equal(numero, end.Numero);
        Assert.Equal(bairro, end.Bairro);
        Assert.Equal(cep, end.Cep);
        Assert.Equal(complemento, end.Complemento);
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Equals_SameInstances_ReturnsTrue_And_HashCodesEqual()
    {
        // Arrange (use same instances to match equality implementation that uses '==' on members)
        var cep = new Cep(12345678);
        var enderecoExterior = new EnderecoExterior(
            new CodigoPaisISO("BR"),
            new CodigoEndPostal("BR123"),
            new NomeCidade("São Paulo"),
            new EstadoProvinciaRegiao("SP")
        );
        var logradouro = new Logradouro("Rua Igual");
        var numero = new NumeroEndereco("10");
        var complemento = new Complemento("A");
        var bairro = new Bairro("Centro");

        var a = new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cep, enderecoExterior, complemento);
        var b = new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cep, enderecoExterior, complemento);

        // Act & Assert
        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Equals_DifferentInstances_ReturnsFalse()
    {
        // Arrange
        var cepA = new Cep(11111111);
        var cepB = new Cep(22222222);
        var logradouro = new Logradouro("Rua X");
        var numero = new NumeroEndereco("1");
        var bairro = new Bairro("Zona");
        var complemento = new Complemento("C");

        var a = new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cepA, null, complemento);
        var b = new EnderecoSimplesIBSCBS(logradouro, numero, bairro, cepB, null, complemento);

        // Act & Assert
        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void EnderecoSimplesIBSCBS_Equals_OtherTypesOrNull_ReturnsFalse_ExceptSelf()
    {
        // Arrange
        var end = new EnderecoSimplesIBSCBS(
            new Logradouro("L"),
            new NumeroEndereco("2"),
            new Bairro("B"),
            new Cep(12345678),
            null,
            null
        );

        // Act & Assert
        Assert.False(end.Equals(null));
        Assert.False(end.Equals("not an endereco"));
        Assert.True(end.Equals(end));
    }
}