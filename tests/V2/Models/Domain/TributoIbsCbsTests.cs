using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class TributoIbsCbsTests
{
    [Fact]
    public void Constructor_WithAllFields_SetsProperties()
    {
        // Arrange
        var classificacao = new ClassificacaoTributaria("123456");
        var tributoRegular = new TributoRegular(new ClassificacaoTributaria("654321"));

        // Act
        var target = new TributoIbsCbs(classificacao, tributoRegular);

        // Assert
        Assert.Equal(classificacao, target.ClassificacaoTributaria);
        Assert.Equal(tributoRegular, target.TributoRegular);
    }

    [Fact]
    public void Constructor_NullClassificacao_Throws()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new TributoIbsCbs(null!, null));
    }

    [Fact]
    public void DefaultConstructor_AllowsNullProperties()
    {
        // Act
        var target = new TributoIbsCbs();

        // Assert
        Assert.Null(target.ClassificacaoTributaria);
        Assert.Null(target.TributoRegular);
    }

    [Fact]
    public void Properties_AreMutable()
    {
        // Arrange
        var target = new TributoIbsCbs();
        var classificacao = new ClassificacaoTributaria("111111");
        var tributoRegular = new TributoRegular(new ClassificacaoTributaria("222222"));

        // Act
        target.ClassificacaoTributaria = classificacao;
        target.TributoRegular = tributoRegular;

        // Assert
        Assert.Equal(classificacao, target.ClassificacaoTributaria);
        Assert.Equal(tributoRegular, target.TributoRegular);
    }

    [Fact]
    public void Xml_RoundTrip_PreservesValues()
    {
        // Arrange
        var original = new TributoIbsCbs(
            new ClassificacaoTributaria("123456"),
            new TributoRegular(new ClassificacaoTributaria("654321")));

        var serializer = new XmlSerializer(typeof(TributoIbsCbs));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, ns);
            xml = sw.ToString();
        }

        // Act
        TributoIbsCbs? deserialized;
        using (var sr = new StringReader(xml))
        {
            deserialized = (TributoIbsCbs?)serializer.Deserialize(sr);
        }

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.ClassificacaoTributaria, deserialized!.ClassificacaoTributaria);
        Assert.Equal(original.TributoRegular?.ClassificacaoTributaria, deserialized.TributoRegular?.ClassificacaoTributaria);
    }

    [Fact]
    public void Xml_Deserialization_WithInvalidClassificacao_ThrowsSerializationExceptionWrapped()
    {
        // Arrange
        var original = new TributoIbsCbs(
            new ClassificacaoTributaria("123456"),
            new TributoRegular(new ClassificacaoTributaria("654321")));

        var serializer = new XmlSerializer(typeof(TributoIbsCbs));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, ns);
            xml = sw.ToString();
        }

        // Corrupt the main ClassificacaoTributaria element value to an invalid value
        xml = xml.Replace("<cClassTrib>123456</cClassTrib>", "<cClassTrib>ABC</cClassTrib>");

        // Act / Assert
        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.NotNull(ex.InnerException);
        Assert.IsType<System.Runtime.Serialization.SerializationException>(ex.InnerException);
    }
}