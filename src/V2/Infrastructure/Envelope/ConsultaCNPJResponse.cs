using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP da consulta de CNPJ (<c>ConsultaCNPJResponse</c>),
/// encapsulando o <see cref="RetornoConsultaCNPJ"/> retornado pelo webservice da NF-e Paulistana v02.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaCNPJResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaCNPJResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaCNPJResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaCNPJResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoConsultaCNPJ"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoConsultaCNPJ>? RetornoXml { get; set; }
}