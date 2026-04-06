using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta da operação de consulta de NFS-e (<c>ConsultaNFeResponse</c>)
/// retornada pelo webservice da NF-e Paulistana v02.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaNFeResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaNFeResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaNFeResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaNFeResponse() { }

    /// <summary>Retorno XML contendo o resultado da consulta.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoConsulta>? RetornoXml { get; set; }
}