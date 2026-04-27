using Microsoft.IO;
using Nfe.Paulistana.Models;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Representa o elemento MensagemXML que encapsula o payload tipado
/// (<typeparamref name="TPayload"/>) dentro de um envelope SOAP da NF-e Paulistana.
/// </summary>
/// <typeparam name="TPayload">
/// Tipo do payload a ser encapsulado. Deve possuir o atributo <see cref="XmlRootAttribute"/>
/// para que o nome do elemento XML seja determinado corretamente durante a serialização.
/// </typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class MensagemXml<TPayload> : IXmlSerializable where TPayload : class
{
    private static readonly XmlSerializer Serializer = new(typeof(TPayload));
    private static readonly XmlSerializerNamespaces EmptyNamespaces = new([XmlQualifiedName.Empty]);

    private static readonly XmlWriterSettings _innerWriterSettings = new()
    {
        Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
        OmitXmlDeclaration = true
    };

    /// <summary>Inicializa uma nova instância de <see cref="MensagemXml{TPayload}"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MensagemXml() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="MensagemXml{TPayload}"/> com o payload especificado.
    /// </summary>
    /// <param name="payload">Payload a ser encapsulado no elemento MensagemXML.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="payload"/> é nulo.</exception>
    public MensagemXml(TPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        Payload = payload;
    }

    /// <summary>Payload encapsulado dentro do elemento MensagemXML.</summary>
    public TPayload? Payload { get; private set; }

    /// <inheritdoc/>
    public XmlSchema? GetSchema() => null;

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        reader.ReadStartElement();

        if (reader.NodeType == XmlNodeType.Element)
        {
            Payload = (TPayload?)Serializer.Deserialize(reader);
        }
        else if (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
        {
            string xmlContent = reader.ReadContentAsString();

            if (!string.IsNullOrWhiteSpace(xmlContent))
            {
                using var innerReader = XmlReader.Create(new StringReader(xmlContent));
                Payload = (TPayload?)Serializer.Deserialize(innerReader);
            }
        }

        reader.ReadEndElement();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// Quando o payload implementa <see cref="ISignedXmlFile"/> e possui
    /// <see cref="ISignedXmlFile.SignedXmlContent"/> definido, o conteúdo XML
    /// pré-assinado é escrito diretamente, garantindo que o XML transmitido seja
    /// idêntico ao que foi assinado digitalmente (evitando erro 1057).
    /// </para>
    /// </remarks>
    public void WriteXml(XmlWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        // Se o payload possui XML pré-assinado, utiliza-o diretamente
        // para garantir que o conteúdo transmitido seja idêntico ao assinado
        if (Payload is ISignedXmlFile signedFile && signedFile.SignedXmlContent != null)
        {
            writer.WriteString(signedFile.SignedXmlContent);
            return;
        }

        using RecyclableMemoryStream ms = StreamManager.Instance.GetStream();
        using var innerWriter = XmlWriter.Create(ms, _innerWriterSettings);
        Serializer.Serialize(innerWriter, Payload, EmptyNamespaces);
        innerWriter.Flush();
        writer.WriteString(Encoding.UTF8.GetString(ms.ToArray()));
    }
}