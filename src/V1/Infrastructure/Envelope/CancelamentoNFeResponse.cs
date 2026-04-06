using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Response;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a resposta SOAP do cancelamento de NFS-e (<c>CancelamentoNFeResponse</c>),
/// encapsulando o <see cref="RetornoCancelamentoNFe"/> retornado pelo webservice da NF-e Paulistana.
/// </summary>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "CancelamentoNFeResponse", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class CancelamentoNFeResponse
{
    /// <summary>Inicializa uma nova instância de <see cref="CancelamentoNFeResponse"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CancelamentoNFeResponse() { }

    /// <summary>Wrapper XML contendo o <see cref="RetornoCancelamentoNFe"/> retornado pelo webservice.</summary>
    [XmlElement("RetornoXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<RetornoCancelamentoNFe>? RetornoXml { get; set; }
}