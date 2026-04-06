using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class OutroDocumentoFiscalTests
{
    [Fact]
    public void DefaultConstructor_PropertiesNull()
    {
        var d = new OutroDocumentoFiscal();

        Assert.Null(d.CodigoMunicipio);
        Assert.Null(d.NumeroDocumento);
        Assert.Null(d.DescricaoDocumento);
    }

    [Fact]
    public void Constructor_WithAllValues_SetsProperties()
    {
        var codigo = new CodigoIbge(3550308);
        var numero = new NumeroDescricaoDocumento("12345");
        var descricao = new NumeroDescricaoDocumento("Descricao");

        var doc = new OutroDocumentoFiscal(codigo, numero, descricao);

        Assert.Equal(codigo, doc.CodigoMunicipio);
        Assert.Equal(numero, doc.NumeroDocumento);
        Assert.Equal(descricao, doc.DescricaoDocumento);
    }

    [Fact]
    public void Constructor_NullCodigo_ThrowsArgumentNullException()
    {
        NumeroDescricaoDocumento numero = new NumeroDescricaoDocumento("1");
        NumeroDescricaoDocumento descricao = new NumeroDescricaoDocumento("d");

        _ = Assert.Throws<ArgumentNullException>(() => new OutroDocumentoFiscal(null!, numero, descricao));
    }

    [Fact]
    public void Constructor_NullNumero_ThrowsArgumentNullException()
    {
        CodigoIbge codigo = new CodigoIbge(3550308);
        NumeroDescricaoDocumento descricao = new NumeroDescricaoDocumento("d");

        _ = Assert.Throws<ArgumentNullException>(() => new OutroDocumentoFiscal(codigo, null!, descricao));
    }

    [Fact]
    public void Constructor_NullDescricao_ThrowsArgumentNullException()
    {
        CodigoIbge codigo = new CodigoIbge(3550308);
        NumeroDescricaoDocumento numero = new NumeroDescricaoDocumento("1");

        _ = Assert.Throws<ArgumentNullException>(() => new OutroDocumentoFiscal(codigo, numero, null!));
    }

    [Fact]
    public void Setters_AssignValues()
    {
        var doc = new OutroDocumentoFiscal();

        doc.CodigoMunicipio = new CodigoIbge(3550308);
        doc.NumeroDocumento = new NumeroDescricaoDocumento("1");
        doc.DescricaoDocumento = new NumeroDescricaoDocumento("d");

        Assert.Equal("3550308", doc.CodigoMunicipio?.ToString());
        Assert.Equal("1", doc.NumeroDocumento?.ToString());
        Assert.Equal("d", doc.DescricaoDocumento?.ToString());
    }

    [Fact]
    public void Xml_SerializeAndDeserialize_RoundTrip()
    {
        var codigo = new CodigoIbge(3550308);
        var numero = new NumeroDescricaoDocumento("12345");
        var descricao = new NumeroDescricaoDocumento("Descricao");

        var original = new OutroDocumentoFiscal(codigo, numero, descricao);

        var serializer = new XmlSerializer(typeof(OutroDocumentoFiscal));
        var namespaces = new XmlSerializerNamespaces(new[] { new System.Xml.XmlQualifiedName() });

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, namespaces);
            xml = sw.ToString();
        }

        OutroDocumentoFiscal? round;
        using (var sr = new StringReader(xml))
        {
            round = (OutroDocumentoFiscal?)serializer.Deserialize(sr);
        }

        Assert.NotNull(round);
        Assert.Equal(original.CodigoMunicipio?.ToString(), round?.CodigoMunicipio?.ToString());
        Assert.Equal(original.NumeroDocumento?.ToString(), round?.NumeroDocumento?.ToString());
        Assert.Equal(original.DescricaoDocumento?.ToString(), round?.DescricaoDocumento?.ToString());
    }
}