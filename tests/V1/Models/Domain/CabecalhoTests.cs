using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class CabecalhoTests
{
    [Fact]
    public void Cabecalho_DefaultConstructor_VersaoIsOne()
    {
        var cab = new Cabecalho();

        Assert.Equal(1, cab.Versao);
        Assert.Null(cab.CpfOrCnpj);
    }

    [Fact]
    public void Cabecalho_WithCpfOrCnpj_SetsCpfOrCnpj()
    {
        var cpfOrCnpj = new CpfOrCnpj((Cpf)Tests.Helpers.TestConstants.ValidCpf);
        var cab = new Cabecalho(cpfOrCnpj);

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
        Assert.Equal(1, cab.Versao);
    }

    [Fact]
    public void Cabecalho_FromCpf_SetsCpfField()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;
        var cab = Cabecalho.FromCpf(cpf);

        Assert.NotNull(cab.CpfOrCnpj);
        Assert.Equal(cpf, cab.CpfOrCnpj.Cpf);
        Assert.Null(cab.CpfOrCnpj.Cnpj);
    }

    [Fact]
    public void Cabecalho_FromCnpj_SetsCnpjField()
    {
        var cnpj = (Cnpj)TestConstants.ValidCnpj;
        var cab = Cabecalho.FromCnpj(cnpj);

        Assert.NotNull(cab.CpfOrCnpj);
        Assert.Equal(cnpj, cab.CpfOrCnpj.Cnpj);
        Assert.Null(cab.CpfOrCnpj.Cpf);
    }

    [Fact]
    public void Cabecalho_ExplicitCastFromCpf_EqualToFromCpf()
    {
        var cpf = (Cpf)Tests.Helpers.TestConstants.ValidCpf;

        var viaFactory = Cabecalho.FromCpf(cpf);
        var viaCast = (Cabecalho)cpf;

        Assert.Equal(viaFactory.CpfOrCnpj?.Cpf, viaCast.CpfOrCnpj?.Cpf);
    }

    [Fact]
    public void Cabecalho_ExplicitCastFromCnpj_EqualToFromCnpj()
    {
        var cnpj = (Cnpj)TestConstants.ValidCnpj;

        var viaFactory = Cabecalho.FromCnpj(cnpj);
        var viaCast = (Cabecalho)cnpj;

        Assert.Equal(viaFactory.CpfOrCnpj?.Cnpj, viaCast.CpfOrCnpj?.Cnpj);
    }
}