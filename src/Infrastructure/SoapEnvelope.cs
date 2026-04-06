using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// <para>
/// Representa o envelope SOAP que encapsula a requisição tipada (<typeparamref name="TRequest"/>)
/// para transmissão ao webservice da NF-e Paulistana.
/// </para>
/// <para>
/// O elemento Body delega a serialização XML ao tipo concreto via <see cref="SoapBody{TRequest}"/>,
/// que implementa <see cref="System.Xml.Serialization.IXmlSerializable"/> para controle preciso do conteúdo.
/// </para>
/// </summary>
/// <typeparam name="TRequest">
/// Tipo da requisição encapsulada no Body SOAP. Deve possuir o atributo
/// <see cref="XmlRootAttribute"/> para determinar o nome do elemento durante a serialização.
/// </typeparam>
/// <remarks>
/// Um envelope SOAP é o contêiner XML para mensagens SOAP. Contém um elemento Body que
/// carrega o payload da operação (ex.: <see cref="EnvioRpsRequest"/> ou <see cref="EnvioLoteRpsRequest"/>).
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Soap)]
[XmlRoot(ElementName = "Envelope", Namespace = Constants.Uris.Soap, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class SoapEnvelope<TRequest> where TRequest : class
{
    /// <summary>Inicializa uma nova instância de <see cref="SoapEnvelope{TRequest}"/>.</summary>
    public SoapEnvelope() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="SoapEnvelope{TRequest}"/> com o Body especificado.
    /// </summary>
    /// <param name="body">Body SOAP contendo a requisição.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="body"/> é nulo.</exception>
    public SoapEnvelope(SoapBody<TRequest> body)
    {
        ArgumentNullException.ThrowIfNull(body);
        Body = body;
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="SoapEnvelope{TRequest}"/>
    /// encapsulando a requisição diretamente.
    /// </summary>
    /// <param name="request">Requisição a ser encapsulada no Body SOAP.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="request"/> é nulo.</exception>
    public SoapEnvelope(TRequest request) : this(new SoapBody<TRequest>(request)) { }

    /// <summary>Body SOAP contendo a requisição tipada.</summary>
    [XmlElement(Namespace = Constants.Uris.Soap)]
    public SoapBody<TRequest>? Body { get; set; }
}