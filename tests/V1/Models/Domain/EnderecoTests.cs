using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class EnderecoTests
{
    [Fact]
    public void Endereco_DefaultConstructor_AllNull()
    {
        var endereco = new Endereco();

        Assert.Null(endereco.Uf);
        Assert.Null(endereco.Cidade);
        Assert.Null(endereco.Bairro);
        Assert.Null(endereco.Cep);
        Assert.Null(endereco.Tipo);
        Assert.Null(endereco.Logradouro);
        Assert.Null(endereco.Numero);
        Assert.Null(endereco.Complemento);
    }

    [Fact]
    public void Endereco_FullConstructor_SetsAllFields()
    {
        var uf = new Uf("SP");
        var cidade = new CodigoIbge(3550308);
        var bairro = new Bairro("Bela Vista");
        var cep = new Cep("01310100");
        var tipo = new TipoLogradouro("Av");
        var logradouro = new Logradouro("Paulista");
        var numero = new NumeroEndereco("1578");
        var complemento = new Complemento("Cj 35");

        var endereco = new Endereco(uf, cidade, bairro, cep, tipo, logradouro, numero, complemento);

        Assert.Equal(uf, endereco.Uf);
        Assert.Equal(cidade, endereco.Cidade);
        Assert.Equal(bairro, endereco.Bairro);
        Assert.Equal(cep, endereco.Cep);
        Assert.Equal(tipo, endereco.Tipo);
        Assert.Equal(logradouro, endereco.Logradouro);
        Assert.Equal(numero, endereco.Numero);
        Assert.Equal(complemento, endereco.Complemento);
    }

    [Fact]
    public void Endereco_PartialConstructor_NullFieldsRemainNull()
    {
        var uf = new Uf("RJ");
        var endereco = new Endereco(uf: uf);

        Assert.Equal(uf, endereco.Uf);
        Assert.Null(endereco.Cidade);
        Assert.Null(endereco.Bairro);
    }
}