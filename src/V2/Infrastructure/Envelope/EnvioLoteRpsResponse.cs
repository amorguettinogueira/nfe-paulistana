using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioLoteRPSResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioLoteRpsResponse
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioLoteRpsResponse() { }

    public EnvioLoteRpsResponse(MensagemXml<RetornoEnvioLoteRps> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        RetornoXml = mensagemXml;
    }

    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoEnvioLoteRps>? RetornoXml { get; set; }

    public static EnvioLoteRpsResponse FromRetornoEnvioLoteRps(RetornoEnvioLoteRps value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<RetornoEnvioLoteRps>(value));
    }

    public static explicit operator EnvioLoteRpsResponse(RetornoEnvioLoteRps value) => FromRetornoEnvioLoteRps(value);
}