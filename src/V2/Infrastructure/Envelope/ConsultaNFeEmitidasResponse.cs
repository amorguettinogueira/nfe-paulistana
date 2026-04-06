using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP da consulta de NFS-e emitidas (<c>ConsultaNFeEmitidasResponse</c>) v02,
/// encapsulando o <see cref="RetornoConsulta"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaNFeEmitidasResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaNFeEmitidasResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaNFeEmitidasResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaNFeEmitidasResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoConsulta"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoConsulta>? RetornoXml { get; set; }
}