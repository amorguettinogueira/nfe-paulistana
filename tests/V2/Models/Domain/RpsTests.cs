using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class RpsTests
{
    [Fact]
    public void DefaultConstructor_InitializesDefaults()
    {
        var rps = new Rps();

        Assert.NotNull(rps.ChaveRps);
        Assert.NotNull(rps.DataEmissao);
        Assert.NotNull(rps.TributacaoRps);
        Assert.NotNull(rps.ValorDeducoes);
        Assert.NotNull(rps.Discriminacao);
    }

    [Fact]
    public void PrimaryConstructor_WithRequiredArgs_SetsProperties()
    {
        var chave = new ChaveRps(new InscricaoMunicipal(39616924), new SerieRps("A"), new Numero(1));
        var tipo = TipoRps.Rps;
        var data = new DataXsd(new DateTime(2024, 1, 1));
        var status = StatusNfe.Normal;
        var tributacao = (TributacaoNfe)'T';
        var valorDeducoes = (Valor)0m;
        var codigoServico = new CodigoServico(7617);
        var aliquota = (Aliquota)0.05m;
        var issRetido = false;
        var discriminacao = new Discriminacao("Serviço");
        var valorPis = (Valor)0m;
        var valorCofins = (Valor)0m;
        var valorInss = (Valor)0m;
        var valorIr = (Valor)0m;
        var valorCsll = (Valor)0m;
        var valorIpi = (Valor)0m;
        var exigibilidade = new NaoSim(false);
        var pagamentoParcelado = new NaoSim(false);
        var nbs = new CodigoNBS("123456789");
        var ibsCbs = new InformacoesIbsCbs(FinalidadeEmissaoNfe.NfseRegular, new NaoSim(false), DestinatarioServicos.ProprioTomador, new Valores(new TributosIbsCbs()), new CodigoOperacaoFornecimento("000001"));

        var rps = new Rps(chave, tipo, data, status, tributacao, valorDeducoes, codigoServico, aliquota, issRetido, discriminacao, valorPis, valorCofins, valorInss, valorIr, valorCsll, valorIpi, exigibilidade, pagamentoParcelado, nbs, ibsCbs);

        Assert.Equal(chave, rps.ChaveRps);
        Assert.Equal(data, rps.DataEmissao);
        Assert.Equal(codigoServico, rps.CodigoServico);
        Assert.Equal(discriminacao, rps.Discriminacao);
    }

    [Fact]
    public void PrimaryConstructor_NullChave_ThrowsArgumentNullException()
    {
        var tipo = TipoRps.Rps;
        var data = new DataXsd(new DateTime(2024, 1, 1));
        var status = StatusNfe.Normal;
        var tributacao = (TributacaoNfe)'T';
        var valorDeducoes = (Valor)0m;
        var codigoServico = new CodigoServico(7617);
        var aliquota = (Aliquota)0.05m;
        var discriminacao = new Discriminacao("S");
        var valorPis = (Valor)0m;
        var valorCofins = (Valor)0m;
        var valorInss = (Valor)0m;
        var valorIr = (Valor)0m;
        var valorCsll = (Valor)0m;
        var valorIpi = (Valor)0m;
        var exigibilidade = new NaoSim(false);
        var pagamentoParcelado = new NaoSim(false);
        var nbs = new CodigoNBS("123456789");
        var ibsCbs = new InformacoesIbsCbs(FinalidadeEmissaoNfe.NfseRegular, new NaoSim(false), DestinatarioServicos.ProprioTomador, new Valores(new TributosIbsCbs()), new CodigoOperacaoFornecimento("000001"));

        _ = Assert.Throws<ArgumentNullException>(() => new Rps(null!, tipo, data, status, tributacao, valorDeducoes, codigoServico, aliquota, false, discriminacao, valorPis, valorCofins, valorInss, valorIr, valorCsll, valorIpi, exigibilidade, pagamentoParcelado, nbs, ibsCbs));
    }

    [Fact]
    public void Xml_SerializeAndDeserialize_RoundTrip_PreservesKeyFields()
    {
        // Build an Rps using the builder for realistic constructed object
        var inscricao = new InscricaoMunicipal(39616924);
        var serie = new SerieRps("BB");
        var numero = new Numero(4105);
        var discriminacao = new Discriminacao("Serviço.");
        var dataEmissao = new DataXsd(new DateTime(2024, 1, 20));
        var tributacao = (TributacaoNfe)'T';
        var codigoServico = new CodigoServico(7617);
        var nbs = new CodigoNBS("123456789");
        var aliquota = (Aliquota)0.05m;
        var ibs = new InformacoesIbsCbs(FinalidadeEmissaoNfe.NfseRegular, new NaoSim(false), DestinatarioServicos.ProprioTomador, new Valores(new TributosIbsCbs()), new CodigoOperacaoFornecimento("000001"));

        var tomador = TomadorBuilder.NewCpf(new Cpf(46381819618)).Build();

        var rps = RpsBuilder.New(inscricao, TipoRps.Rps, numero, discriminacao, serie)
            .SetNFe(dataEmissao, tributacao, new NaoSim(false), new NaoSim(false))
            .SetServico(codigoServico, nbs)
            .SetIss(aliquota, false)
            .SetIbsCbs(ibs)
            .SetValorInicialCobrado((Valor)1000m)
            .SetLocalPrestacao((CodigoIbge)1234567)
            .SetTomador(tomador)
            .Build();

        var serializer = new XmlSerializer(typeof(Rps));
        var namespaces = new XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty });

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, rps, namespaces);
            xml = sw.ToString();
        }

        Rps? round;
        using (var sr = new StringReader(xml))
        {
            round = (Rps?)serializer.Deserialize(sr);
        }

        Assert.NotNull(round);
        Assert.Equal(rps.ChaveRps?.InscricaoPrestador?.ToString(), round?.ChaveRps?.InscricaoPrestador?.ToString());
        Assert.Equal(rps.ChaveRps?.NumeroRps?.ToString(), round?.ChaveRps?.NumeroRps?.ToString());
        Assert.Equal(rps.DataEmissao?.ToString(), round?.DataEmissao?.ToString());
        Assert.Equal(rps.CodigoServico?.ToString(), round?.CodigoServico?.ToString());
    }
}
