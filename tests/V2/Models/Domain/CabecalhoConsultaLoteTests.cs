using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

/// <summary>
/// Testes unitários para <see cref="CabecalhoConsultaLote"/> (V2).
/// </summary>
public class CabecalhoConsultaLoteTests
{
    [Fact]
    public void DefaultConstructor_PropriedadesNulas()
    {
        var cab = new CabecalhoConsultaLote();

        Assert.Null(cab.CpfOrCnpj);
        Assert.Null(cab.NumeroLote);
        Assert.Equal(2, cab.Versao);
    }

    [Fact]
    public void Constructor_ComCpfOrCnpj_DefinePropriedade()
    {
        var cpfOrCnpj = new CpfOrCnpj((Cpf)Tests.Helpers.TestConstants.ValidCpf);
        var cab = new CabecalhoConsultaLote(cpfOrCnpj);

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
        Assert.Null(cab.NumeroLote);
    }

    [Fact]
    public void NumeroLote_QuandoDefinido_RetornaMesmaReferencia()
    {
        var numero = new Numero(12345);
        var cab = new CabecalhoConsultaLote { NumeroLote = numero };

        Assert.Equal(numero, cab.NumeroLote);
    }

    [Fact]
    public void HerdaDeCabecalho() =>
        Assert.True(typeof(CabecalhoConsultaLote).IsSubclassOf(typeof(Cabecalho)));

    [Fact]
    public void Constructor_ComCpfENumeroLote_DefineAmbasPropriedades()
    {
        var cpfOrCnpj = new CpfOrCnpj((Cpf)Tests.Helpers.TestConstants.ValidCpf);
        var numero = new Numero(42);

        var cab = new CabecalhoConsultaLote(cpfOrCnpj) { NumeroLote = numero };

        Assert.Equal(cpfOrCnpj, cab.CpfOrCnpj);
        Assert.Equal(numero, cab.NumeroLote);
    }
}