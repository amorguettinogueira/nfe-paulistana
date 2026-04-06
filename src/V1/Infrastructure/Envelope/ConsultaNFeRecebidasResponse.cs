using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP da consulta de NFS-e recebidas (<c>ConsultaNFeRecebidasResponse</c>),
/// encapsulando o <see cref="RetornoConsulta"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaNFeRecebidasResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaNFeRecebidasResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaNFeRecebidasResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaNFeRecebidasResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoConsulta"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoConsulta>? RetornoXml { get; set; }
}