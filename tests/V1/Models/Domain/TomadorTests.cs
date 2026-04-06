using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class TomadorTests
{
    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Tomador_WithAllFields_SetsAllProperties(long cpfNumber)
    {
        var cpfOrCnpj = new CpfOrCnpj(new Cpf(cpfNumber));
        var razaoSocial = new RazaoSocial("Empresa Teste Ltda");
        var inscricaoMunicipal = new InscricaoMunicipal(12345678);
        var inscricaoEstadual = new InscricaoEstadual(123456789);
        var email = new Email("tomador@teste.com.br");
        Endereco endereco = EnderecoBuilder.New().SetUf(new Uf("SP")).Build();

        var tomador = new Tomador(cpfOrCnpj, razaoSocial, inscricaoMunicipal, inscricaoEstadual, email, endereco);

        Assert.Equal(cpfOrCnpj, tomador.CpfOrCnpjTomador);
        Assert.Equal(razaoSocial, tomador.RazaoSocialTomador);
        Assert.Equal(inscricaoMunicipal, tomador.InscricaoMunicipalTomador);
        Assert.Equal(inscricaoEstadual, tomador.InscricaoEstadualTomador);
        Assert.Equal(email, tomador.EmailTomador);
        Assert.Equal(endereco, tomador.EnderecoTomador);
    }

    [Fact]
    public void Tomador_WithOnlyRazaoSocial_NullOptionalFieldsAccepted()
    {
        var razaoSocial = new RazaoSocial("Empresa Teste Ltda");

        var tomador = new Tomador(null, razaoSocial, null, null, null, null);

        Assert.Null(tomador.CpfOrCnpjTomador);
        Assert.Equal(razaoSocial, tomador.RazaoSocialTomador);
        Assert.Null(tomador.InscricaoMunicipalTomador);
        Assert.Null(tomador.InscricaoEstadualTomador);
        Assert.Null(tomador.EmailTomador);
        Assert.Null(tomador.EnderecoTomador);
    }

    [Fact]
    public void Tomador_PropertiesAreMutable()
    {
        var tomador = new Tomador(null, new RazaoSocial("Original"), null, null, null, null);
        var novaRazao = new RazaoSocial("Atualizada");

        tomador.RazaoSocialTomador = novaRazao;

        Assert.Equal(novaRazao, tomador.RazaoSocialTomador);
    }
}