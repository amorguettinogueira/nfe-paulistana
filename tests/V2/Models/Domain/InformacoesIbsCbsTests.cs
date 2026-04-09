using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class InformacoesIbsCbsTests
{
    [Fact]
    public void DefaultConstructor_PropertiesHaveDefaultsOrNull()
    {
        var info = new InformacoesIbsCbs();

        Assert.Equal(FinalidadeEmissaoNfe.NfseRegular, info.FinalidadeEmissao);
        Assert.Null(info.UsoOuConsumoPessoal);
        Assert.Null(info.CodigoOperacaoFornecimento);
        Assert.Null(info.TipoOperacao);
        Assert.Null(info.NfesReferenciadas);
        Assert.Null(info.EnteGovernamental);
        Assert.Equal(DestinatarioServicos.ProprioTomador, info.DestinatarioServicos);
        Assert.Null(info.Destinatario);
        Assert.Null(info.Valores);
        Assert.Null(info.ImovelObra);
    }

    [Fact]
    public void Constructor_WithRequiredArguments_SetsProperties()
    {
        var uso = new NaoSim(true);
        var codigo = new CodigoOperacao("000001");
        var valores = new Valores(new TributosIbsCbs());

        var info = new InformacoesIbsCbs(
            FinalidadeEmissaoNfe.NfseRegular,
            uso,
            DestinatarioServicos.OutraPessoa,
            valores,
            codigo
        );

        Assert.Equal(FinalidadeEmissaoNfe.NfseRegular, info.FinalidadeEmissao);
        Assert.Equal(uso, info.UsoOuConsumoPessoal);
        Assert.Equal(codigo, info.CodigoOperacaoFornecimento);
        Assert.Equal(DestinatarioServicos.OutraPessoa, info.DestinatarioServicos);
        Assert.Equal(valores, info.Valores);
    }

    [Fact]
    public void Constructor_NullUsoOuConsumo_ThrowsArgumentNullException()
    {
        NaoSim? uso = null;
        var codigo = new CodigoOperacao("000001");
        var valores = new Valores(new TributosIbsCbs());

        _ = Assert.Throws<ArgumentNullException>(() => new InformacoesIbsCbs(FinalidadeEmissaoNfe.NfseRegular, uso!, DestinatarioServicos.ProprioTomador, valores, codigo));
    }

    [Fact]
    public void Constructor_NullCodigoOperacao_ThrowsArgumentNullException()
    {
        CodigoOperacao? codigo = null;
        var uso = new NaoSim(false);
        var valores = new Valores(new TributosIbsCbs());

        _ = Assert.Throws<ArgumentNullException>(() => new InformacoesIbsCbs(FinalidadeEmissaoNfe.NfseRegular, uso, DestinatarioServicos.ProprioTomador, valores, codigo!));
    }

    [Fact]
    public void Constructor_NullValores_ThrowsArgumentNullException()
    {
        Valores? valores = null;
        var uso = new NaoSim(false);
        var codigo = new CodigoOperacao("000001");

        _ = Assert.Throws<ArgumentNullException>(() => new InformacoesIbsCbs(FinalidadeEmissaoNfe.NfseRegular, uso, DestinatarioServicos.ProprioTomador, valores!, codigo));
    }

    [Theory]
    [ClassData(typeof(ValidCpfNumber))]
    public void Setters_AssignValuesCorrectly(long cpfNumber)
    {
        var info = new InformacoesIbsCbs();

        info.FinalidadeEmissao = FinalidadeEmissaoNfe.NfseRegular;
        info.UsoOuConsumoPessoal = new NaoSim(true);
        info.CodigoOperacaoFornecimento = new CodigoOperacao("000002");
        info.TipoOperacao = TipoOperacao.FornecimentoComPagamentoRealizado;
        info.TipoOperacaoSpecified = true;
        info.NfesReferenciadas = new GrupoNfeReferenciada(new[] { new ChaveNotaNacional(new string('A', 40) + "1234567890") });
        info.EnteGovernamental = TipoEnteGovernamental.Municipios;
        info.EnteGovernamentalSpecified = true;
        info.DestinatarioServicos = DestinatarioServicos.OutraPessoa;
        info.Destinatario = new InformacoesPessoa((Cpf)cpfNumber, new RazaoSocial("Nome"));
        info.Valores = new Valores(new TributosIbsCbs());
        var imovel = new ImovelObra(new CadastroImovel("ABCDEFGH"));
        info.ImovelObra = imovel;

        Assert.Equal(FinalidadeEmissaoNfe.NfseRegular, info.FinalidadeEmissao);
        Assert.Equal("1", info.UsoOuConsumoPessoal?.ToString());
        Assert.Equal("000002", info.CodigoOperacaoFornecimento?.ToString());
        Assert.Equal(TipoOperacao.FornecimentoComPagamentoRealizado, info.TipoOperacao);
        Assert.True(info.TipoOperacaoSpecified);
        Assert.NotNull(info.NfesReferenciadas);
        Assert.Equal(TipoEnteGovernamental.Municipios, info.EnteGovernamental);
        Assert.True(info.EnteGovernamentalSpecified);
        Assert.Equal(DestinatarioServicos.OutraPessoa, info.DestinatarioServicos);
        Assert.NotNull(info.Destinatario);
        Assert.NotNull(info.Valores);
        Assert.Equal(imovel, info.ImovelObra);
    }
}