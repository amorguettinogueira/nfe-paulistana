using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioLoteRPSRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioLoteRpsRequest
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioLoteRpsRequest() { }

    public EnvioLoteRpsRequest(MensagemXml<PedidoEnvioLote> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    public int VersaoSchema { get; set; } = 2;

    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoEnvioLote>? MensagemXml { get; set; }

    public static EnvioLoteRpsRequest FromPedidoEnvioLote(PedidoEnvioLote value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoEnvioLote>(value));
    }

    public static explicit operator EnvioLoteRpsRequest(PedidoEnvioLote value) => FromPedidoEnvioLote(value);
}