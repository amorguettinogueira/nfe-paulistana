using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Builders;

public class EnderecoBuilderTests
{
    [Fact]
    public void New_WithoutParameters_ReturnsBuilder()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New();
        Assert.NotNull(builder);
        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(builder);
    }

    [Fact]
    public void New_WithAllFields_ReturnsBuilderWithFields()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New(
            uf: new Uf("SP"),
            cidade: new CodigoIbge(3550308),
            bairro: new Bairro("Centro"),
            cep: new Cep("01310100"),
            tipo: new TipoLogradouro("Av"),
            logradouro: new Logradouro("Paulista"),
            numero: new NumeroEndereco("1000"),
            complemento: new Complemento("Apto 1"),
            codigoPais: new CodigoPaisISO("BR"),
            codigoEndereco: new CodigoEndPostal("12345"),
            nomeCidade: new NomeCidade("São Paulo"),
            estadoProvinciaRegiao: new EstadoProvinciaRegiao("SP")
        );
        Assert.NotNull(builder);
        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(builder);
    }

    [Fact]
    public void SetUf_WithValidValue_ReturnsBuilder()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New().SetUf(new Uf("SP"));
        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(builder);
    }

    [Fact]
    public void SetUf_WithNull_ThrowsArgumentNullException()
    {
        Uf? uf = null;
        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetUf(uf!));
    }

    [Fact]
    public void SetEnderecoExterior_WithAllFields_ReturnsBuilder()
    {
        IEnderecoBuilder builder = EnderecoBuilder.New().SetEnderecoExterior(
            new CodigoPaisISO("BR"),
            new CodigoEndPostal("12345"),
            new NomeCidade("São Paulo"),
            new EstadoProvinciaRegiao("SP")
        );
        _ = Assert.IsAssignableFrom<IEnderecoBuilder>(builder);
    }

    [Fact]
    public void SetEnderecoExterior_WithNullFields_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetEnderecoExterior(null!, new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP")));
        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetEnderecoExterior(new CodigoPaisISO("BR"), null!, new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP")));
        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), null!, new EstadoProvinciaRegiao("SP")));
        _ = Assert.Throws<ArgumentNullException>(() => EnderecoBuilder.New().SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), null!));
    }

    [Fact]
    public void IsValid_WithNoFields_ReturnsFalse() =>
        Assert.False(EnderecoBuilder.New().IsValid());

    [Fact]
    public void IsValid_WithOneField_ReturnsTrue() =>
        Assert.True(EnderecoBuilder.New().SetUf(new Uf("SP")).IsValid());

    [Fact]
    public void GetValidationErrors_WithNoFields_ReturnsSingleError()
    {
        var errors = EnderecoBuilder.New().GetValidationErrors().ToList();
        Assert.Single(errors);
    }

    [Fact]
    public void Build_WithNoFields_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => EnderecoBuilder.New().Build());

    [Fact]
    public void Build_WithAllFields_ReturnsEnderecoWithAllProperties()
    {
        Endereco endereco = EnderecoBuilder.New()
            .SetUf(new Uf("SP"))
            .SetCodigoIbge(new CodigoIbge(3550308))
            .SetBairro(new Bairro("Centro"))
            .SetCep(new Cep("01310100"))
            .SetTipo(new TipoLogradouro("Av"))
            .SetLogradouro(new Logradouro("Paulista"))
            .SetNumero(new NumeroEndereco("1000"))
            .SetComplemento(new Complemento("Apto 1"))
            .SetEnderecoExterior(new CodigoPaisISO("BR"), new CodigoEndPostal("12345"), new NomeCidade("São Paulo"), new EstadoProvinciaRegiao("SP"))
            .Build();
        Assert.NotNull(endereco);
        Assert.NotNull(endereco.Uf);
        Assert.NotNull(endereco.Cidade);
        Assert.NotNull(endereco.Bairro);
        Assert.NotNull(endereco.Cep);
        Assert.NotNull(endereco.Tipo);
        Assert.NotNull(endereco.Logradouro);
        Assert.NotNull(endereco.Numero);
        Assert.NotNull(endereco.Complemento);
        Assert.NotNull(endereco.EnderecoExterior);
    }
}