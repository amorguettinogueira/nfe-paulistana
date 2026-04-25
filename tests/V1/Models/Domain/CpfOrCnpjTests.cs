using Nfe.Paulistana.Models.DataTypes;
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

    [Fact]
    public void CpfOrCnpj_WithCpf_SetsCpfAndLeavesNullCnpj()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Fact]
    public void CpfOrCnpj_WithCnpj_SetsCnpjAndLeavesNullCpf()
    {
        var cnpj = (Cnpj)TestConstants.ValidCnpj;
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

    [Fact]
    public void CpfOrCnpj_ToString_ReturnsCpfWhenCpfIsSet()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = new CpfOrCnpj(cpf);

        Assert.Equal(cpf.ToString(), doc.ToString());
    }

    [Fact]
    public void CpfOrCnpj_ToString_ReturnsCnpjWhenCnpjIsSet()
    {
        var cnpj = (Cnpj)TestConstants.ValidCnpj;
        var doc = new CpfOrCnpj(cnpj);

        Assert.Equal(cnpj.ToString(), doc.ToString());
    }

    [Fact]
    public void CpfOrCnpj_ToString_ReturnsNullWhenBothNull() =>
        Assert.Null(new CpfOrCnpj().ToString());

    [Fact]
    public void CpfOrCnpj_FromCpf_CreatesCpfInstance()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var doc = CpfOrCnpj.FromCpf(cpf);

        Assert.Equal(cpf, doc.Cpf);
        Assert.Null(doc.Cnpj);
    }

    [Fact]
    public void CpfOrCnpj_FromCnpj_CreatesCnpjInstance()
    {
        var cnpj = (Cnpj)TestConstants.ValidCnpj;
        var doc = CpfOrCnpj.FromCnpj(cnpj);

        Assert.Equal(cnpj, doc.Cnpj);
        Assert.Null(doc.Cpf);
    }

    [Fact]
    public void CpfOrCnpj_ExplicitCastFromCpf_EqualToFromCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;

        var viaFactory = CpfOrCnpj.FromCpf(cpf);
        var viaCast = (CpfOrCnpj)cpf;

        Assert.Equal(viaFactory.Cpf, viaCast.Cpf);
    }

    [Fact]
    public void CpfOrCnpj_ExplicitCastFromCnpj_EqualToFromCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidCnpj;

        var viaFactory = CpfOrCnpj.FromCnpj(cnpj);
        var viaCast = (CpfOrCnpj)cnpj;

        Assert.Equal(viaFactory.Cnpj, viaCast.Cnpj);
    }
}