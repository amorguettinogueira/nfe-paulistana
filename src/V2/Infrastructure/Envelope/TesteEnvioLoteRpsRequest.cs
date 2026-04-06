using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "TesteEnvioLoteRPSRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class TesteEnvioLoteRpsRequest
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TesteEnvioLoteRpsRequest() { }

    public TesteEnvioLoteRpsRequest(MensagemXml<PedidoEnvioLote> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    public int VersaoSchema { get; set; } = 2;

    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoEnvioLote>? MensagemXml { get; set; }

    public static TesteEnvioLoteRpsRequest FromPedidoEnvioLote(PedidoEnvioLote value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoEnvioLote>(value));
    }

    public static explicit operator TesteEnvioLoteRpsRequest(PedidoEnvioLote value) => FromPedidoEnvioLote(value);
}