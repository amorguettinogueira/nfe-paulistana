using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class FornecedorTests
{
    [Fact]
    public void Fornecedor_DefaultConstructor_AllPropertiesNull()
    {
        var f = new Fornecedor();

        Assert.Null(f.Cpf);
        Assert.Null(f.Cnpj);
        Assert.Null(f.Nif);
        Assert.Null(f.Nome);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Fornecedor_WithCpfAndNome_SetsProperties(long cpfNumber)
    {
        var cpf = (Cpf)cpfNumber;
        var nome = new RazaoSocial("Nome Fornecedor");

        var f = new Fornecedor(cpf, nome);

        Assert.Equal(cpf, f.Cpf);
        Assert.Null(f.Cnpj);
        Assert.Null(f.Nif);
        Assert.Equal(nome, f.Nome);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void Fornecedor_WithCnpjAndNome_SetsProperties(string cnpjFormatted, string _)
    {
        var cnpj = new Cnpj(cnpjFormatted);
        var nome = new RazaoSocial("Empresa");

        var f = new Fornecedor(cnpj, nome);

        Assert.Equal(cnpj, f.Cnpj);
        Assert.Null(f.Cpf);
        Assert.Null(f.Nif);
        Assert.Equal(nome, f.Nome);
    }

    [Fact]
    public void Fornecedor_WithNifAndNome_SetsProperties()
    {
        var nif = new Nif("NIF123");
        var nome = new RazaoSocial("Fornecedor Estrangeiro");

        var f = new Fornecedor(nif, nome);

        Assert.Equal(nif, f.Nif);
        Assert.Null(f.Cpf);
        Assert.Null(f.Cnpj);
        Assert.Equal(nome, f.Nome);
    }

    [Fact]
    public void Fornecedor_WithMotivoNifNaoInformadoAndNome_SetsProperties()
    {
        const MotivoNifNaoInformado motivo = MotivoNifNaoInformado.Dispensado;
        var nome = new RazaoSocial("Fornecedor Sem NIF");

        var f = new Fornecedor(motivo, nome);

        Assert.Equal(motivo, f.MotivoNifNaoInformado);
        Assert.Null(f.Cpf);
        Assert.Null(f.Cnpj);
        Assert.Null(f.Nif);
        Assert.Equal(nome, f.Nome);
        Assert.True(f.MotivoNifNaoInformadoSpecified);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Fornecedor_Ctor_NullNome_ThrowsArgumentNullException_ForCpf(long cpfNumber)
    {
        RazaoSocial? nome = null;
        var cpf = (Cpf)cpfNumber;

        _ = Assert.Throws<ArgumentNullException>(() => new Fornecedor(cpf, nome!));
    }

    [Theory]
    [ClassData(typeof(ValidCnpjString))]
    public void Fornecedor_Ctor_NullNome_ThrowsArgumentNullException_ForCnpj(string cnpjFormatted, string _)
    {
        RazaoSocial? nome = null;
        var cnpj = new Cnpj(cnpjFormatted);

        Assert.Throws<ArgumentNullException>(() => new Fornecedor(cnpj, nome!));
    }

    [Fact]
    public void Fornecedor_Ctor_NullNome_ThrowsArgumentNullException_ForNif()
    {
        RazaoSocial? nome = null;
        var nif = new Nif("123");

        _ = Assert.Throws<ArgumentNullException>(() => new Fornecedor(nif, nome!));
    }
}