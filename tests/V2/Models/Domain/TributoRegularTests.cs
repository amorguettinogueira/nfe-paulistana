using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class TributoRegularTests
{
    [Fact]
    public void Constructor_WithClassificacao_SetsProperty()
    {
        // Arrange
        var classificacao = new ClassificacaoTributaria("123456");

        // Act
        var target = new TributoRegular(classificacao);

        // Assert
        Assert.Equal(classificacao, target.ClassificacaoTributaria);
    }

    [Fact]
    public void Constructor_NullClassificacao_Throws()
    {
        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new TributoRegular(null!));
    }

    [Fact]
    public void DefaultConstructor_AllowsNullProperty()
    {
        // Act
        var target = new TributoRegular();

        // Assert
        Assert.Null(target.ClassificacaoTributaria);
    }

    [Fact]
    public void Property_IsMutable()
    {
        // Arrange
        var target = new TributoRegular();
        var classificacao = new ClassificacaoTributaria("111111");

        // Act
        target.ClassificacaoTributaria = classificacao;

        // Assert
        Assert.Equal(classificacao, target.ClassificacaoTributaria);

        // Act - set to null
        target.ClassificacaoTributaria = null;

        // Assert
        Assert.Null(target.ClassificacaoTributaria);
    }

    [Fact]
    public void Xml_RoundTrip_PreservesClassificacao()
    {
        // Arrange
        var original = new TributoRegular(new ClassificacaoTributaria("654321"));
        var serializer = new XmlSerializer(typeof(TributoRegular));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, ns);
            xml = sw.ToString();
        }

        // Act
        TributoRegular? deserialized;
        using (var sr = new StringReader(xml))
        {
            deserialized = (TributoRegular?)serializer.Deserialize(sr);
        }

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.ClassificacaoTributaria, deserialized!.ClassificacaoTributaria);
    }

    [Fact]
    public void Xml_Deserialization_WithInvalidClassificacao_ThrowsSerializationExceptionWrapped()
    {
        // Arrange
        var original = new TributoRegular(new ClassificacaoTributaria("654321"));
        var serializer = new XmlSerializer(typeof(TributoRegular));
        var ns = new XmlSerializerNamespaces();
        ns.Add(string.Empty, string.Empty);

        string xml;
        using (var sw = new StringWriter())
        {
            serializer.Serialize(sw, original, ns);
            xml = sw.ToString();
        }

        // Corrupt the classificacao element
        xml = xml.Replace("<cClassTribReg>654321</cClassTribReg>", "<cClassTribReg>ABC</cClassTribReg>");

        // Act / Assert
        using var sr = new StringReader(xml);
        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));
        Assert.NotNull(ex.InnerException);
        Assert.IsType<System.Runtime.Serialization.SerializationException>(ex.InnerException);
    }
}