using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class CpfOrCnpjTests
{
    [Fact]
    public void CpfOrCnpj_DefaultConstructor_BothNull()
    {
        var doc = new CpfOrCnpj();

        Assert.Null(doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void CpfOrCnpj_WithCpf_SetsCpfAndLeavesNullCnpj(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void CpfOrCnpj_WithCnpj_SetsCnpjAndLeavesNullCpf(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
        var doc = new CpfOrCnpj(cnpj);

        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
    }

    [Fact]
    public void CpfOrCnpj_WithNullCpf_ThrowsArgumentNullException()
    {
        Cpf? cpf = null;

        _ = Assert.Throws<ArgumentNullException>(() => new CpfOrCnpj(cpf!));
    }

    [Fact]
    public void CpfOrCnpj_WithNullCnpj_ThrowsArgumentNullException()
    {
        Cnpj? cnpj = null;

        _ = Assert.Throws<ArgumentNullException>(() => new CpfOrCnpj(cnpj!));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void CpfOrCnpj_ToString_ReturnsCpfWhenCpfIsSet(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf.ToString(), doc.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void CpfOrCnpj_ToString_ReturnsCnpjWhenCnpjIsSet(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
        var doc = new CpfOrCnpj(cnpj);

        Assert.Equal(cnpj.ToString(), doc.ToString());
    }

    [Fact]
    public void CpfOrCnpj_ToString_ReturnsNullWhenBothNull() =>
        Assert.Null(new CpfOrCnpj().ToString());

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void CpfOrCnpj_FromCpf_CreatesCpfInstance(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var doc = CpfOrCnpj.FromCpf(cpf);

        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void CpfOrCnpj_FromCnpj_CreatesCnpjInstance(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
        var doc = CpfOrCnpj.FromCnpj(cnpj);

        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void CpfOrCnpj_ExplicitCastFromCpf_EqualToFromCpf(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);

        var viaFactory = CpfOrCnpj.FromCpf(cpf);
        var viaCast = (CpfOrCnpj)cpf;

        Assert.Equal(viaFactory.Cpf, viaCast.Cpf);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void CpfOrCnpj_ExplicitCastFromCnpj_EqualToFromCnpj(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);

        var viaFactory = CpfOrCnpj.FromCnpj(cnpj);
        var viaCast = (CpfOrCnpj)cnpj;

        Assert.Equal(viaFactory.Cnpj, viaCast.Cnpj);
    }
}