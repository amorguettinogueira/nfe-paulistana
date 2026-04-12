using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="CabecalhoLote"/> (V2).
/// </summary>
public sealed class CabecalhoLoteTests
{
    [Fact]
    public void DefaultConstructor_TransacaoTrueEVersaoDois()
    {
        var cab = new CabecalhoLote();

        Assert.True(cab.Transacao);
        Assert.Equal(2, cab.Versao);
        Assert.Null(cab.CpfOrCnpj);
        Assert.Null(cab.DtInicio);
        Assert.Null(cab.DtFim);
        Assert.Null(cab.QtdRps);
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Constructor_ComCpfOrCnpj_DefiniCpfOrCnpj(long cpfNumber)
    {
        var cpfOrCnpj = new CpfOrCnpj(new Cpf(cpfNumber));
        var cab = new CabecalhoLote(cpfOrCnpj);

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
    }

    [Fact]
    public void CabecalhoLote_InheritsFromCabecalho() =>
        Assert.True(typeof(CabecalhoLote).IsSubclassOf(typeof(Cabecalho)));

    [Fact]
    public void CabecalhoLote_VersaoEDois() =>
        Assert.Equal(2, new CabecalhoLote().Versao);

    [Fact]
    public void CabecalhoLote_ComCamposOpcionais_PropagaCorretamente()
    {
        var inicio = new DataXsd(new DateTime(2024, 1, 1));
        var fim = new DataXsd(new DateTime(2024, 1, 31));
        var qtd = new Quantidade(5);

        var cab = new CabecalhoLote
        {
            DtInicio = inicio,
            DtFim = fim,
            QtdRps = qtd,
            Transacao = false
        };

        Assert.Equal(inicio, cab.DtInicio);
        Assert.Equal(fim, cab.DtFim);
        Assert.Equal(qtd, cab.QtdRps);
        Assert.False(cab.Transacao);
    }

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CabecalhoLote).IsSealed);
}
