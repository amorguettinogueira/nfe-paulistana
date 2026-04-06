using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class ChaveRpsTests
{
    [Fact]
    public void ChaveRps_DefaultConstructor_AllNull()
    {
        var chave = new ChaveRps();

        Assert.Null(chave.InscricaoPrestador);
        Assert.Null(chave.SerieRps);
        Assert.Null(chave.NumeroRps);
    }

    [Fact]
    public void ChaveRps_FullConstructor_SetsAllFields()
    {
        var inscricao = new InscricaoMunicipal(39616924);
        var serie = new SerieRps("BB");
        var numero = new Numero(4105);

        var chave = new ChaveRps(inscricao, serie, numero);

        Assert.Equal(inscricao, chave.InscricaoPrestador);
        Assert.Equal(serie, chave.SerieRps);
        Assert.Equal(numero, chave.NumeroRps);
    }
}