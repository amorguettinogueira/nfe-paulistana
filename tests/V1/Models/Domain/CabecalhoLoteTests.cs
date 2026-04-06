using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class CabecalhoLoteTests
{
    [Fact]
    public void CabecalhoLote_DefaultConstructor_TransacaoTrueAndVersaoOne()
    {
        var cab = new CabecalhoLote();

        Assert.True(cab.Transacao);
        Assert.Equal(1, cab.Versao);
        Assert.Null(cab.CpfOrCnpj);
        Assert.Null(cab.DtInicio);
        Assert.Null(cab.DtFim);
        Assert.Null(cab.QtdRps);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void CabecalhoLote_WithCpfOrCnpj_SetsCpfOrCnpj(long cpfNumber)
    {
        var cpfOrCnpj = new CpfOrCnpj(new Cpf(cpfNumber));
        var cab = new CabecalhoLote(cpfOrCnpj);

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
    }

    [Fact]
    public void CabecalhoLote_InheritsFromCabecalho() =>
        Assert.True(typeof(CabecalhoLote).IsSubclassOf(typeof(Cabecalho)));

    [Fact]
    public void CabecalhoLote_WithAllFields_PropagatesCorrectly()
    {
        var inicio = new DataXsd(new DateTime(2024, 1, 1));
        var fim = new DataXsd(new DateTime(2024, 1, 31));
        var qtd = new Quantidade(5);

        var cab = new CabecalhoLote
        {
            DtInicio = inicio,
            DtFim = fim,
            QtdRps = qtd,
            ValorTotalServicos = (Valor)10000m,
            ValorTotalDeducoes = (Valor)500m,
            Transacao = false
        };

        Assert.Equal(inicio, cab.DtInicio);
        Assert.Equal(fim, cab.DtFim);
        Assert.Equal(qtd, cab.QtdRps);
        Assert.Equal(((Valor)10000m).ToString(), cab.ValorTotalServicos.ToString());
        Assert.Equal(((Valor)500m).ToString(), cab.ValorTotalDeducoes.ToString());
        Assert.False(cab.Transacao);
    }
}