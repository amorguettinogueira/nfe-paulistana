using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP do envio de lote de RPS em modo de teste (<c>TesteEnvioLoteRPSResponse</c>),
/// encapsulando o <see cref="RetornoEnvioLoteRps"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "TesteEnvioLoteRPSResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class TesteEnvioLoteRpsResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="TesteEnvioLoteRpsResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TesteEnvioLoteRpsResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoEnvioLoteRps"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoEnvioLoteRps>? RetornoXml { get; set; }
}