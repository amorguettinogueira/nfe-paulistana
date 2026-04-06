using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class DocumentoFiscalNacionalTests
{
    [Fact]
    public void DefaultConstructor_AllPropertiesNull()
    {
        var doc = new DocumentoFiscalNacional();
        Assert.Null(doc.TipoDocumentoFiscal);
        Assert.Null(doc.TipoChaveDfe);
        Assert.Null(doc.ChaveDocumentoFiscal);
        Assert.False(doc.TipoDocumentoFiscalSpecified);
    }

    [Fact]
    public void Constructor_ValidArguments_PropertiesSetCorrectly()
    {
        var chave = new ChaveDocumentoFiscal("12345678901234567890123456789012345678901234567890");
        var doc = new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfe, chave);
        Assert.Equal(TipoDocumentoFiscal.Nfe, doc.TipoDocumentoFiscal);
        Assert.Equal(chave, doc.ChaveDocumentoFiscal);
        Assert.Null(doc.TipoChaveDfe);
    }

    [Fact]
    public void Constructor_TipoDocumentoFiscalOutroWithoutTipoChaveDfe_ThrowsArgumentException()
    {
        var chave = new ChaveDocumentoFiscal("12345678901234567890123456789012345678901234567890");
        var ex = Assert.Throws<ArgumentException>(() => new DocumentoFiscalNacional(TipoDocumentoFiscal.Outro, chave));
        Assert.Contains("tipoChaveDfe", ex.Message);
    }

    [Fact]
    public void Constructor_NullChaveDocumentoFiscal_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new DocumentoFiscalNacional(TipoDocumentoFiscal.Nfe, null!));

    [Fact]
    public void Constructor_TipoDocumentoFiscalOutroWithTipoChaveDfe_SetsPropertiesCorrectly()
    {
        var chave = new ChaveDocumentoFiscal("12345678901234567890123456789012345678901234567890");
        var tipoChave = new TipoChaveDfe("Outro tipo");
        var doc = new DocumentoFiscalNacional(TipoDocumentoFiscal.Outro, chave, tipoChave);
        Assert.Equal(TipoDocumentoFiscal.Outro, doc.TipoDocumentoFiscal);
        Assert.Equal(chave, doc.ChaveDocumentoFiscal);
        Assert.Equal(tipoChave, doc.TipoChaveDfe);
    }
}