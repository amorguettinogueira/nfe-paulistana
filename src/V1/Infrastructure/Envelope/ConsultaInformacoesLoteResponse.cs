using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP da consulta de informações de lote (<c>ConsultaInformacoesLoteResponse</c>),
/// encapsulando o <see cref="RetornoInformacoesLote"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaInformacoesLoteResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaInformacoesLoteResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaInformacoesLoteResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaInformacoesLoteResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoInformacoesLote"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoInformacoesLote>? RetornoXml { get; set; }
}