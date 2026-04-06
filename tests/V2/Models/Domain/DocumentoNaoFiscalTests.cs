using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class DocumentoNaoFiscalTests
{
    [Fact]
    public void DefaultConstructor_PropertiesAreNull()
    {
        var doc = new DocumentoNaoFiscal();
        Assert.Null(doc.NumeroDocumento);
        Assert.Null(doc.DescricaoDocumento);
    }

    [Fact]
    public void ParameterizedConstructor_ValidArguments_PropertiesSetCorrectly()
    {
        var numero = new NumeroDescricaoDocumento("12345");
        var descricao = new NumeroDescricaoDocumento("Nota de Serviço");
        var doc = new DocumentoNaoFiscal(numero, descricao);
        Assert.Equal(numero, doc.NumeroDocumento);
        Assert.Equal(descricao, doc.DescricaoDocumento);
    }

    [Fact]
    public void ParameterizedConstructor_NullNumeroDocumento_ThrowsArgumentNullException()
    {
        NumeroDescricaoDocumento? numero = null;
        var descricao = new NumeroDescricaoDocumento("Nota de Serviço");
        Action act = () => new DocumentoNaoFiscal(numero!, descricao);
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("numeroDocumento", ex.ParamName);
    }

    [Fact]
    public void ParameterizedConstructor_NullDescricaoDocumento_ThrowsArgumentNullException()
    {
        var numero = new NumeroDescricaoDocumento("12345");
        NumeroDescricaoDocumento? descricao = null;
        Action act = () => new DocumentoNaoFiscal(numero, descricao!);
        var ex = Assert.Throws<ArgumentNullException>(act);
        Assert.Equal("descricaoDocumento", ex.ParamName);
    }

    [Fact]
    public void NumeroDocumento_Setter_Works()
    {
        var doc = new DocumentoNaoFiscal();
        var numero = new NumeroDescricaoDocumento("54321");
        doc.NumeroDocumento = numero;
        Assert.Equal(numero, doc.NumeroDocumento);
    }

    [Fact]
    public void DescricaoDocumento_Setter_Works()
    {
        var doc = new DocumentoNaoFiscal();
        var descricao = new NumeroDescricaoDocumento("Recibo");
        doc.DescricaoDocumento = descricao;
        Assert.Equal(descricao, doc.DescricaoDocumento);
    }
}