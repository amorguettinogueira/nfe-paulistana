using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "TesteEnvioLoteRPSResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class TesteEnvioLoteRpsResponse
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TesteEnvioLoteRpsResponse() { }

    public TesteEnvioLoteRpsResponse(MensagemXml<RetornoEnvioLoteRps> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        RetornoXml = mensagemXml;
    }

    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoEnvioLoteRps>? RetornoXml { get; set; }

    public static TesteEnvioLoteRpsResponse FromRetornoEnvioLoteRps(RetornoEnvioLoteRps value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<RetornoEnvioLoteRps>(value));
    }

    public static explicit operator TesteEnvioLoteRpsResponse(RetornoEnvioLoteRps value) => FromRetornoEnvioLoteRps(value);
}