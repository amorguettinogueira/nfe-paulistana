using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class TributosIbsCbsTests
{
    [Fact]
    public void Constructor_WithTributoIbsCbs_SetsProperty()
    {
        // Arrange
        var inner = new TributoIbsCbs(new ClassificacaoTributaria("123456"), null);

        // Act
        var target = new TributosIbsCbs(inner);

        // Assert
        Assert.Equal(inner, target.TributoIbsCbs);
    }

    [Fact]
    public void Constructor_NullTributoIbsCbs_Throws()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new TributosIbsCbs(null!));
    }

    [Fact]
    public void DefaultConstructor_AllowsNullProperty()
    {
        // Act
        var target = new TributosIbsCbs();

        // Assert
        Assert.Null(target.TributoIbsCbs);
    }

    [Fact]
    public void Property_IsMutable()
    {
        // Arrange
        var target = new TributosIbsCbs();
        var inner = new TributoIbsCbs(new ClassificacaoTributaria("111111"), null);

        // Act
        target.TributoIbsCbs = inner;

        // Assert
        Assert.Equal(inner, target.TributoIbsCbs);

        // Act - set to null
        target.TributoIbsCbs = null;

        // Assert
        Assert.Null(target.TributoIbsCbs);
    }

    [Fact]
    public void Xml_RoundTrip_PreservesNestedTributo()
    {
        // Arrange
        var original = new TributosIbsCbs(new TributoIbsCbs(new ClassificacaoTributaria("123456"), new TributoRegular(new ClassificacaoTributaria("654321"))));
        var serializer = new XmlSerializer(typeof(TributosIbsCbs));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, ns);
            xml = sw.ToString();
        }

        // Act
        TributosIbsCbs? deserialized;
        using (var sr = new StringReader(xml))
        {
            deserialized = (TributosIbsCbs?)serializer.Deserialize(sr);
        }

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.TributoIbsCbs?.ClassificacaoTributaria, deserialized!.TributoIbsCbs?.ClassificacaoTributaria);
        Assert.Equal(original.TributoIbsCbs?.TributoRegular?.ClassificacaoTributaria, deserialized.TributoIbsCbs?.TributoRegular?.ClassificacaoTributaria);
    }

    [Fact]
    public void Xml_Deserialization_WithInvalidNestedClassificacao_ThrowsSerializationExceptionWrapped()
    {
        // Arrange
        var original = new TributosIbsCbs(new TributoIbsCbs(new ClassificacaoTributaria("123456"), new TributoRegular(new ClassificacaoTributaria("654321"))));
        var serializer = new XmlSerializer(typeof(TributosIbsCbs));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, ns);
            xml = sw.ToString();
        }

        // Corrupt the nested classificacao element inside TributoIbsCbs
        xml = xml.Replace("<cClassTrib>123456</cClassTrib>", "<cClassTrib>BAD</cClassTrib>");

        // Act / Assert
        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.NotNull(ex.InnerException);
        Assert.IsType<System.Runtime.Serialization.SerializationException>(ex.InnerException);
    }
}