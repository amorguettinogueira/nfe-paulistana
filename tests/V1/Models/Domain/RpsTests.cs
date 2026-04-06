using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;

namespace Nfe.Paulistana.Tests.V1.Models.Domain;

public class RpsTests
{
    [Fact]
    public void Rps_Build_ChaveRpsContainsInscricaoAndSerieAndNumero()
    {
        var inscricao = new InscricaoMunicipal(39616924);
        var serie = new SerieRps("BB");
        var numero = new Numero(4105);
        var tomador = new Tomador(null, new RazaoSocial("Tomador"), null, null, null, null);

        Rps rps = RpsBuilder
            .New(inscricao, TipoRps.Rps, numero, new Discriminacao("Serviço."), serie)
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T')
            .SetServico(new CodigoServico(7617), (Valor)1000m)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(tomador)
            .Build();

        Assert.Equal(inscricao.ToString(), rps.ChaveRps?.InscricaoPrestador?.ToString());
        Assert.Equal(serie.ToString(), rps.ChaveRps?.SerieRps?.ToString());
        Assert.Equal(numero.ToString(), rps.ChaveRps?.NumeroRps?.ToString());
    }

    [Fact]
    public void Rps_Build_ValorDeducoesDefaultsToZeroWhenOmitted()
    {
        var tomador = new Tomador(null, new RazaoSocial("Tomador"), null, null, null, null);

        Rps rps = RpsBuilder
            .New(new InscricaoMunicipal(39616924), TipoRps.Rps,
                 new Numero(1), new Discriminacao("Serviço."), new SerieRps("A"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T')
            .SetServico(new CodigoServico(7617), (Valor)500m)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(tomador)
            .Build();

        Assert.Equal(((Valor)0).ToString(), rps.ValorDeducoes?.ToString());
    }
}