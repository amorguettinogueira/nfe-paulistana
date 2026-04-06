using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP do envio de RPS unitário (<c>EnvioRPSResponse</c>),
/// encapsulando o <see cref="RetornoEnvioRps"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioRPSResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioRpsResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="EnvioRpsResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioRpsResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoEnvioRps"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoEnvioRps>? RetornoXml { get; set; }
}