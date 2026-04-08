using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="CabecalhoCancelamento"/> (V1).
/// </summary>
public class CabecalhoCancelamentoTests
{
    [Fact]
    public void DefaultConstructor_TransacaoTrueEVersaoUm()
    {
        var cab = new CabecalhoCancelamento();

        Assert.True(cab.Transacao);
        Assert.Equal(1, cab.Versao);
        Assert.Null(cab.CpfOrCnpj);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Constructor_ComCpfOrCnpj_DefinePropriedade(long cpfNumber)
    {
        var cpfOrCnpj = new CpfOrCnpj((Cpf)cpfNumber);
        var cab = new CabecalhoCancelamento(cpfOrCnpj);

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
        Assert.True(cab.Transacao);
    }

    [Fact]
    public void Transacao_QuandoDefinidaComoFalse_RetornaFalse()
    {
        var cab = new CabecalhoCancelamento { Transacao = false };

        Assert.False(cab.Transacao);
    }

    [Fact]
    public void Transacao_QuandoDefinidaComoNull_RetornaNull()
    {
        var cab = new CabecalhoCancelamento { Transacao = null };

        Assert.Null(cab.Transacao);
    }

    [Fact]
    public void HerdaDeCabecalho() =>
        Assert.True(typeof(CabecalhoCancelamento).IsSubclassOf(typeof(Cabecalho)));

    [Theory]
    [ClassData(typeof(ValidCnpjNumber))]
    public void Constructor_ComCnpj_DefinePropriedade(long cnpjNumber)
    {
        var cnpj = new Cnpj(cnpjNumber);
        var cpfOrCnpj = new CpfOrCnpj(cnpj);
        var cab = new CabecalhoCancelamento(cpfOrCnpj);

        Assert.Equal(cnpj, cab.CpfOrCnpj!.Cnpj);
        Assert.Null(cab.CpfOrCnpj.Cpf);
    }
}
