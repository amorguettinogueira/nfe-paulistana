using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Representa o elemento Body do envelope SOAP, encapsulando a requisição tipada
/// (<typeparamref name="TRequest"/>) para transmissão ao webservice da NF-e Paulistana.
/// </summary>
/// <typeparam name="TRequest">
/// Tipo da requisição a ser encapsulada. Deve possuir o atributo <see cref="XmlRootAttribute"/>
/// para que o nome do elemento XML seja determinado corretamente durante a serialização.
/// </typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class SoapBody<TRequest> : IXmlSerializable where TRequest : class
{
    private static readonly XmlSerializer Serializer = new(typeof(TRequest));
    private static readonly XmlSerializerNamespaces EmptyNamespaces = new([XmlQualifiedName.Empty]);

    /// <summary>Inicializa uma nova instância de <see cref="SoapBody{TRequest}"/>.</summary>
    public SoapBody() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="SoapBody{TRequest}"/> com a requisição especificada.
    /// </summary>
    /// <param name="request">Requisição a ser encapsulada no Body SOAP.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="request"/> é nulo.</exception>
    public SoapBody(TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        Request = request;
    }

    /// <summary>Requisição encapsulada no Body SOAP.</summary>
    public TRequest? Request { get; private set; }

    /// <inheritdoc/>
    public XmlSchema? GetSchema() => null;

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        reader.ReadStartElement();

        if (!reader.EOF && reader.NodeType != XmlNodeType.EndElement)
        {
            Request = (TRequest?)Serializer.Deserialize(reader);
        }

        reader.ReadEndElement();
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        Serializer.Serialize(writer, Request, EmptyNamespaces);
    }
}