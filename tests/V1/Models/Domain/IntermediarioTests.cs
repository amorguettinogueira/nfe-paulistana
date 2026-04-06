using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class IntermediarioTests
{
    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Intermediario_WithCpf_SetsAllFields(long cpfNumber)
    {
        var cpfOrCnpj = new CpfOrCnpj((Cpf)cpfNumber);
        var inscricao = new InscricaoMunicipal(12345678);
        var email = new Email("inter@teste.com.br");

        var intermediario = new Intermediario(cpfOrCnpj, inscricao, true, email);

        Assert.Equal(cpfOrCnpj, intermediario.CpfCnpjIntermediario);
        Assert.Equal(inscricao, intermediario.InscricaoMunicipalIntermediario);
        Assert.True(intermediario.IssRetidoIntermediario);
        Assert.Equal(email, intermediario.EmailIntermediario);
    }

    [Fact]
    public void Intermediario_WithNullOptionals_NullFieldsAccepted()
    {
        var inscricao = new InscricaoMunicipal(12345678);

        var intermediario = new Intermediario(null, inscricao, false, null);

        Assert.Null(intermediario.CpfCnpjIntermediario);
        Assert.Equal(inscricao, intermediario.InscricaoMunicipalIntermediario);
        Assert.False(intermediario.IssRetidoIntermediario);
        Assert.Null(intermediario.EmailIntermediario);
    }

    [Fact]
    public void Intermediario_PropertiesAreMutable()
    {
        var intermediario = new Intermediario(null, new InscricaoMunicipal(12345678), false, null);
        var novoEmail = new Email("novo@teste.com.br");

        intermediario.EmailIntermediario = novoEmail;

        Assert.Equal(novoEmail, intermediario.EmailIntermediario);
    }
}