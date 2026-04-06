using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class DocumentoTests
{
    [Fact]
    public void Documento_ValidFiscalNacional_CreatesInstance()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(100m);
        var fiscalNacional = new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfse, new ChaveDocumentoFiscal("123"));

        var doc = new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            valor,
            fiscalNacional,
            null,
            null,
            null,
            null);

        Assert.Equal(dataEmissao, doc.DataEmissao);
        Assert.Equal(dataCompetencia, doc.DataCompetencia);
        Assert.Equal(valor, doc.ValorDocumento);
        Assert.Equal(fiscalNacional, doc.DocumentoFiscalNacional);
        Assert.Null(doc.OutroDocumentoFiscal);
        Assert.Null(doc.DocumentoNaoFiscal);
        Assert.Null(doc.Fornecedor);
        Assert.Null(doc.DescricaoOutrosReembolsos);
    }

    [Fact]
    public void Documento_ValidOutroDocumentoFiscal_CreatesInstance()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(200m);
        var outroDoc = new OutroDocumentoFiscal(new CodigoIbge(3550308), new NumeroDescricaoDocumento("123"), new NumeroDescricaoDocumento("desc"));

        var doc = new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.RepasseFornecedorTurismo,
            valor,
            null,
            outroDoc,
            null,
            null,
            null);

        Assert.Equal(outroDoc, doc.OutroDocumentoFiscal);
        Assert.Null(doc.DocumentoFiscalNacional);
        Assert.Null(doc.DocumentoNaoFiscal);
    }

    [Fact]
    public void Documento_ValidDocumentoNaoFiscal_CreatesInstance()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(300m);
        var naoFiscal = new DocumentoNaoFiscal(new NumeroDescricaoDocumento("456"), new NumeroDescricaoDocumento("desc2"));

        var doc = new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.ReembolsoServicoProducaoExterna,
            valor,
            null,
            null,
            naoFiscal,
            null,
            null);

        Assert.Equal(naoFiscal, doc.DocumentoNaoFiscal);
        Assert.Null(doc.DocumentoFiscalNacional);
        Assert.Null(doc.OutroDocumentoFiscal);
    }

    [Fact]
    public void Documento_NullDataEmissao_ThrowsArgumentNullException()
    {
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(100m);
        _ = Assert.Throws<ArgumentNullException>(() => new Documento(
            null!,
            dataCompetencia,
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            valor,
            new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfse, new ChaveDocumentoFiscal("123")),
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void Documento_NullDataCompetencia_ThrowsArgumentNullException()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var valor = new Valor(100m);
        _ = Assert.Throws<ArgumentNullException>(() => new Documento(
            dataEmissao,
            null!,
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            valor,
            new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfse, new ChaveDocumentoFiscal("123")),
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void Documento_NullValorDocumento_ThrowsArgumentNullException()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        _ = Assert.Throws<ArgumentNullException>(() => new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            null!,
            new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfse, new ChaveDocumentoFiscal("123")),
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void Documento_AllDocumentTypesNull_ThrowsArgumentException()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(100m);
        _ = Assert.Throws<ArgumentException>(() => new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            valor,
            null,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void Documento_OutrosReembolsosWithoutDescricao_ThrowsArgumentException()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(100m);
        var fiscalNacional = new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfse, new ChaveDocumentoFiscal("123"));
        _ = Assert.Throws<ArgumentException>(() => new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.OutrosReembolsos,
            valor,
            fiscalNacional,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void Documento_OutrosReembolsosWithDescricao_CreatesInstance()
    {
        var dataEmissao = new DataXsd(DateTime.Today);
        var dataCompetencia = new DataXsd(DateTime.Today);
        var valor = new Valor(100m);
        var fiscalNacional = new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfse, new ChaveDocumentoFiscal("123"));
        var descricao = new DescricaoOutrosReembolsos("Teste outros reembolsos");
        var doc = new Documento(
            dataEmissao,
            dataCompetencia,
            TipoValorIncluso.OutrosReembolsos,
            valor,
            fiscalNacional,
            null,
            null,
            null,
            descricao);
        Assert.Equal(descricao, doc.DescricaoOutrosReembolsos);
    }
}