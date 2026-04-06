using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
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

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Cabecalho_WithCpfOrCnpj_SetsCpfOrCnpj(long cpfNumber)
    {
        var cpfOrCnpj = new CpfOrCnpj(new Cpf(cpfNumber));
        var cab = new Cabecalho(cpfOrCnpj);

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
        Assert.Equal(1, cab.Versao);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Cabecalho_FromCpf_SetsCpfField(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);
        var cab = Cabecalho.FromCpf(cpf);

        Assert.NotNull(cab.CpfOrCnpj);
        Assert.Equal(cpf, cab.CpfOrCnpj.Cpf);
        Assert.Null(cab.CpfOrCnpj.Cnpj);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void Cabecalho_FromCnpj_SetsCnpjField(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
        var cab = Cabecalho.FromCnpj(cnpj);

        Assert.NotNull(cab.CpfOrCnpj);
        Assert.Equal(cnpj, cab.CpfOrCnpj.Cnpj);
        Assert.Null(cab.CpfOrCnpj.Cpf);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Cabecalho_ExplicitCastFromCpf_EqualToFromCpf(long cpfNumber)
    {
        var cpf = new Cpf(cpfNumber);

        var viaFactory = Cabecalho.FromCpf(cpf);
        var viaCast = (Cabecalho)cpf;

        Assert.Equal(viaFactory.CpfOrCnpj?.Cpf, viaCast.CpfOrCnpj?.Cpf);
    }

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void Cabecalho_ExplicitCastFromCnpj_EqualToFromCnpj(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);

        var viaFactory = Cabecalho.FromCnpj(cnpj);
        var viaCast = (Cabecalho)cnpj;

        Assert.Equal(viaFactory.CpfOrCnpj?.Cnpj, viaCast.CpfOrCnpj?.Cnpj);
    }
}