using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class ValoresTests
{
    [Fact]
    public void Constructor_WithTributosAndGrupo_SetsProperties()
    {
        // Arrange
        var documento = new Documento(
            new DataXsd(new DateTime(2024, 1, 1)),
            new DataXsd(new DateTime(2024, 1, 1)),
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            new Valor(100m),
            new DocumentoFiscalNacional(),
            null,
            null,
            null,
            null);

        var grupo = new GrupoValorIncluso(new[] { documento });
        var tributos = new TributosIbsCbs(new TributoIbsCbs(new ClassificacaoTributaria("123456"), null));

        // Act
        var target = new Valores(tributos, grupo);

        // Assert
        Assert.Equal(tributos, target.TributosIbsCbs);
        Assert.Equal(grupo, target.GrupoValoresInclusos);
    }

    [Fact]
    public void Constructor_NullTributos_Throws()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new Valores(null!, null));
    }

    [Fact]
    public void DefaultConstructor_AllowsNullProperties()
    {
        // Act
        var target = new Valores();

        // Assert
        Assert.Null(target.TributosIbsCbs);
        Assert.Null(target.GrupoValoresInclusos);
    }

    [Fact]
    public void Properties_AreMutable()
    {
        // Arrange
        var target = new Valores();
        var documento = new Documento(
            new DataXsd(new DateTime(2024, 1, 1)),
            new DataXsd(new DateTime(2024, 1, 1)),
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            new Valor(100m),
            new DocumentoFiscalNacional(),
            null,
            null,
            null,
            null);
        var grupo = new GrupoValorIncluso(new[] { documento });
        var tributos = new TributosIbsCbs(new TributoIbsCbs(new ClassificacaoTributaria("111111"), null));

        // Act
        target.GrupoValoresInclusos = grupo;
        target.TributosIbsCbs = tributos;

        // Assert
        Assert.Equal(grupo, target.GrupoValoresInclusos);
        Assert.Equal(tributos, target.TributosIbsCbs);

        // Act - set to null
        target.GrupoValoresInclusos = null;
        target.TributosIbsCbs = null;

        // Assert
        Assert.Null(target.GrupoValoresInclusos);
        Assert.Null(target.TributosIbsCbs);
    }

    [Fact]
    public void Xml_RoundTrip_PreservesNestedValues()
    {
        // Arrange
        var documento = new Documento(
            new DataXsd(new DateTime(2024, 1, 1)),
            new DataXsd(new DateTime(2024, 1, 1)),
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            new Valor(100m),
            new DocumentoFiscalNacional(),
            null,
            null,
            null,
            null);

        var grupo = new GrupoValorIncluso([documento]);
        var valores = new Valores(
            new TributosIbsCbs(new TributoIbsCbs(new ClassificacaoTributaria("123456"), new TributoRegular(new ClassificacaoTributaria("654321")))),
            grupo);

        var serializer = new XmlSerializer(typeof(Valores));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, valores, ns);
            xml = sw.ToString();
        }

        // Act
        Valores? deserialized;
        using (var sr = new StringReader(xml))
        {
            deserialized = (Valores?)serializer.Deserialize(sr);
        }

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(valores.TributosIbsCbs?.TributoIbsCbs?.ClassificacaoTributaria, deserialized!.TributosIbsCbs?.TributoIbsCbs?.ClassificacaoTributaria);
        Assert.Equal(valores.TributosIbsCbs?.TributoIbsCbs?.TributoRegular?.ClassificacaoTributaria, deserialized.TributosIbsCbs?.TributoIbsCbs?.TributoRegular?.ClassificacaoTributaria);
        Assert.NotNull(deserialized.GrupoValoresInclusos);
        Assert.NotNull(deserialized.GrupoValoresInclusos!.Documentos);
        Assert.Single(deserialized.GrupoValoresInclusos.Documentos);
        Assert.Equal(documento.DataEmissao, deserialized.GrupoValoresInclusos.Documentos[0].DataEmissao);
        Assert.Equal(documento.DataCompetencia, deserialized.GrupoValoresInclusos.Documentos[0].DataCompetencia);
        Assert.Equal(documento.TipoValorIncluso, deserialized.GrupoValoresInclusos.Documentos[0].TipoValorIncluso);
        Assert.Equal(documento.ValorDocumento, deserialized.GrupoValoresInclusos.Documentos[0].ValorDocumento);
        Assert.NotNull(deserialized.GrupoValoresInclusos.Documentos[0].DocumentoFiscalNacional);
    }

    [Fact]
    public void Xml_Deserialization_WithInvalidNestedClassificacao_ThrowsSerializationExceptionWrapped()
    {
        // Arrange
        var valores = new Valores(
            new TributosIbsCbs(new TributoIbsCbs(new ClassificacaoTributaria("123456"), new TributoRegular(new ClassificacaoTributaria("654321")))),
            null);

        var serializer = new XmlSerializer(typeof(Valores));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, valores, ns);
            xml = sw.ToString();
        }

        // Corrupt the nested classificacao value
        xml = xml.Replace("<cClassTrib>123456</cClassTrib>", "<cClassTrib>BAD</cClassTrib>");

        // Act / Assert
        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.NotNull(ex.InnerException);
        Assert.IsType<System.Runtime.Serialization.SerializationException>(ex.InnerException);
    }
}