using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.Models;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Infrastructure;

public class MensagemXmlTests
{
    [Fact]
    public void Constructor_WithoutParameters_CreatesInstanceWithNullPayload()
    {
        // Arrange & Act
        var mensagem = new MensagemXml<TestPayload>();

        // Assert
        Assert.Null(mensagem.Payload);
    }

    [Fact]
    public void Constructor_WithPayload_SetsPayloadCorrectly()
    {
        // Arrange
        var payload = new TestPayload { Value = "Test" };

        // Act
        var mensagem = new MensagemXml<TestPayload>(payload);

        // Assert
        Assert.NotNull(mensagem.Payload);
        Assert.Equal("Test", mensagem.Payload.Value);
    }

    [Fact]
    public void Constructor_WithNullPayload_ThrowsArgumentNullException()
    {
        // Arrange
        TestPayload? payload = null;

        // Act & Assert
        _ = Assert.ThrowsAny<ArgumentNullException>(() => new MensagemXml<TestPayload>(payload!));
    }

    [Fact]
    public void GetSchema_Always_ReturnsNull()
    {
        // Arrange
        var mensagem = new MensagemXml<TestPayload>();

        // Act
        var schema = mensagem.GetSchema();

        // Assert
        Assert.Null(schema);
    }

    [Fact]
    public void ReadXml_WithElementNode_DeserializesPayloadCorrectly()
    {
        // Arrange
        var xml = @"<MensagemXML><TestPayload><Value>ReadTest</Value></TestPayload></MensagemXML>";
        var mensagem = new MensagemXml<TestPayload>();

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader);

        // Act
        mensagem.ReadXml(xmlReader);

        // Assert
        Assert.NotNull(mensagem.Payload);
        Assert.Equal("ReadTest", mensagem.Payload.Value);
    }

    [Fact]
    public void ReadXml_WithStringContent_DeserializesPayloadCorrectly()
    {
        // Arrange
        var xml = @"<MensagemXML>&lt;TestPayload&gt;&lt;Value&gt;StringContent&lt;/Value&gt;&lt;/TestPayload&gt;</MensagemXML>";
        var mensagem = new MensagemXml<TestPayload>();

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader);

        // Act
        mensagem.ReadXml(xmlReader);

        // Assert
        Assert.NotNull(mensagem.Payload);
        Assert.Equal("StringContent", mensagem.Payload.Value);
    }

    [Fact]
    public void ReadXml_WithEmptyContent_LeavesPayloadNull()
    {
        // Arrange
        var xml = @"<MensagemXML></MensagemXML>";
        var mensagem = new MensagemXml<TestPayload>();

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader);

        // Act
        mensagem.ReadXml(xmlReader);

        // Assert
        Assert.Null(mensagem.Payload);
    }

    [Fact]
    public void ReadXml_WithWhitespaceContent_LeavesPayloadNull()
    {
        // Arrange
        var xml = @"<MensagemXML>   </MensagemXML>";
        var mensagem = new MensagemXml<TestPayload>();

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader);

        // Act
        mensagem.ReadXml(xmlReader);

        // Assert
        Assert.Null(mensagem.Payload);
    }

    [Fact]
    public void ReadXml_WithNullReader_ThrowsArgumentNullException()
    {
        // Arrange
        var mensagem = new MensagemXml<TestPayload>();
        XmlReader? reader = null;

        // Act & Assert
        _ = Assert.ThrowsAny<ArgumentNullException>(() => mensagem.ReadXml(reader!));
    }

    [Fact]
    public void WriteXml_WithPayloadWithoutSignature_SerializesCorrectly()
    {
        // Arrange
        var payload = new TestPayload { Value = "WriteTest" };
        var mensagem = new MensagemXml<TestPayload>(payload);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);

        // Act
        xmlWriter.WriteStartElement("Root");
        mensagem.WriteXml(xmlWriter);
        xmlWriter.WriteEndElement();
        xmlWriter.Flush();

        var result = stringWriter.ToString();

        // Assert
        Assert.Contains("TestPayload", result);
        Assert.Contains("WriteTest", result);
    }

    [Fact]
    public void WriteXml_WithSignedPayload_WritesSignedContent()
    {
        // Arrange
        var signedContent = "<TestPayload><Value>Signed</Value><Signature>...</Signature></TestPayload>";
        var payload = new SignedTestPayload
        {
            Value = "Signed",
            SignedXmlContent = signedContent
        };
        var mensagem = new MensagemXml<SignedTestPayload>(payload);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);

        // Act
        xmlWriter.WriteStartElement("Root");
        mensagem.WriteXml(xmlWriter);
        xmlWriter.WriteEndElement();
        xmlWriter.Flush();

        var result = stringWriter.ToString();

        // Assert - O conteúdo é escrito como string escapada, não como XML estruturado
        Assert.Contains("&lt;TestPayload&gt;", result);
        Assert.Contains("&lt;Value&gt;Signed&lt;/Value&gt;", result);
        Assert.Contains("&lt;Signature&gt;", result);
    }

    [Fact]
    public void WriteXml_WithSignedPayloadButNullSignedContent_SerializesNormally()
    {
        // Arrange
        var payload = new SignedTestPayload
        {
            Value = "NotSigned",
            SignedXmlContent = null
        };
        var mensagem = new MensagemXml<SignedTestPayload>(payload);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);

        // Act
        xmlWriter.WriteStartElement("Root");
        mensagem.WriteXml(xmlWriter);
        xmlWriter.WriteEndElement();
        xmlWriter.Flush();

        var result = stringWriter.ToString();

        // Assert
        Assert.Contains("SignedTestPayload", result);
        Assert.Contains("NotSigned", result);
    }

    [Fact]
    public void WriteXml_WithNullWriter_ThrowsArgumentNullException()
    {
        // Arrange
        var payload = new TestPayload { Value = "Test" };
        var mensagem = new MensagemXml<TestPayload>(payload);
        XmlWriter? writer = null;

        // Act & Assert
        _ = Assert.ThrowsAny<ArgumentNullException>(() => mensagem.WriteXml(writer!));
    }

    // Classes auxiliares para testes
    [XmlRoot("TestPayload")]
    public class TestPayload
    {
        public string? Value { get; set; }
    }

    [XmlRoot("SignedTestPayload")]
    public class SignedTestPayload : ISignedXmlFile
    {
        public string? Value { get; set; }
        public string? SignedXmlContent { get; set; }
    }
}
