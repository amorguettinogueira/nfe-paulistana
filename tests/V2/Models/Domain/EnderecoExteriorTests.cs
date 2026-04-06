using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class EnderecoExteriorTests
{
    [Fact]
    public void EnderecoExterior_DefaultConstructor_AllPropertiesNull()
    {
        var endereco = new EnderecoExterior();

        Assert.Null(endereco.CodigoPais);
        Assert.Null(endereco.CodigoEndereco);
        Assert.Null(endereco.NomeCidade);
        Assert.Null(endereco.EstadoProvinciaRegiao);
    }

    [Fact]
    public void EnderecoExterior_Constructor_WithAllValues_SetsProperties()
    {
        var pais = new CodigoPaisISO("US");
        var cep = new CodigoEndPostal("12345");
        var cidade = new NomeCidade("New York");
        var estado = new EstadoProvinciaRegiao("NY");

        var endereco = new EnderecoExterior(pais, cep, cidade, estado);

        Assert.Equal(pais, endereco.CodigoPais);
        Assert.Equal(cep, endereco.CodigoEndereco);
        Assert.Equal(cidade, endereco.NomeCidade);
        Assert.Equal(estado, endereco.EstadoProvinciaRegiao);
    }

    [Fact]
    public void EnderecoExterior_Constructor_AnyNullParameter_ThrowsArgumentNullException()
    {
        var pais = new CodigoPaisISO("US");
        var cep = new CodigoEndPostal("12345");
        var cidade = new NomeCidade("New York");
        var estado = new EstadoProvinciaRegiao("NY");

        _ = Assert.Throws<ArgumentNullException>(() => _ = new EnderecoExterior(null!, cep, cidade, estado));
        _ = Assert.Throws<ArgumentNullException>(() => _ = new EnderecoExterior(pais, null!, cidade, estado));
        _ = Assert.Throws<ArgumentNullException>(() => _ = new EnderecoExterior(pais, cep, null!, estado));
        _ = Assert.Throws<ArgumentNullException>(() => _ = new EnderecoExterior(pais, cep, cidade, null!));
    }

    [Fact]
    public void EnderecoExterior_Equals_SameValues_ReturnsTrue_And_HashCodesEqual()
    {
        var a = new EnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("BR123"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"));
        var b = new EnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("BR123"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"));

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void EnderecoExterior_Equals_DifferentValues_ReturnsFalse()
    {
        var commonPais = new CodigoPaisISO("FR");
        var a = new EnderecoExterior(commonPais, new CodigoEndPostal("1"), new NomeCidade("Paris"), new EstadoProvinciaRegiao("Ile-de-France"));
        var b = new EnderecoExterior(commonPais, new CodigoEndPostal("2"), new NomeCidade("Lyon"), new EstadoProvinciaRegiao("Auvergne-Rhone-Alpes"));

        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void EnderecoExterior_Equals_OtherTypesOrNull_ReturnsFalse_ExceptSelf()
    {
        var endereco = new EnderecoExterior(new CodigoPaisISO("JP"), new CodigoEndPostal("000"), new NomeCidade("Tokyo"), new EstadoProvinciaRegiao("Kanto"));

        Assert.False(endereco.Equals(null));
        Assert.False(endereco.Equals("not an endereco"));
        Assert.True(endereco.Equals(endereco));
    }
}