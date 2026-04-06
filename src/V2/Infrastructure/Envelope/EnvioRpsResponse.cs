using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta da operação de envio de RPS (<c>EnvioRPSResponse</c>) retornada
/// pelo webservice da NF-e Paulistana v02.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioRPSResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioRpsResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="EnvioRpsResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioRpsResponse() { }

    /// <summary>Retorno XML contendo o resultado do envio.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoEnvioRps>? RetornoXml { get; set; }
}