using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP do envio de lote de RPS em modo de produção (<c>EnvioLoteRPSResponse</c>),
/// encapsulando o <see cref="RetornoEnvioLoteRps"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioLoteRPSResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioLoteRpsResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="EnvioLoteRpsResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioLoteRpsResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoEnvioLoteRps"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoEnvioLoteRps>? RetornoXml { get; set; }
}