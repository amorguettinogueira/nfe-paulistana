using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Detalhe do pedido de consulta de NFS-e, contendo a chave de identificação
/// de uma NFS-e ou de um RPS. Representa o elemento inline <c>Detalhe</c>
/// de <c>PedidoConsultaNFe</c>.
/// </summary>
/// <remarks>
/// Fonte: <c>PedidoConsultaNFe_v01.xsd</c> — Elemento <c>Detalhe</c> (1–50 ocorrências).
/// Exatamente uma das propriedades <see cref="ChaveRps"/> ou <see cref="ChaveNfe"/> deve ser definida.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[Serializable]
public sealed class DetalheConsultaNFe
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DetalheConsultaNFe() { }

    /// <summary>
    /// Inicializa o detalhe com uma chave de RPS.
    /// Mutuamente exclusivo com <see cref="ChaveNfe"/>.
    /// </summary>
    /// <param name="chaveRps">Chave do RPS a consultar.</param>
    public DetalheConsultaNFe(ChaveRps chaveRps) => ChaveRps = chaveRps;

    /// <summary>
    /// Inicializa o detalhe com uma chave de NFS-e.
    /// Mutuamente exclusivo com <see cref="ChaveRps"/>.
    /// </summary>
    /// <param name="chaveNfe">Chave da NFS-e a consultar.</param>
    public DetalheConsultaNFe(ChaveNfe chaveNfe) => ChaveNfe = chaveNfe;

    /// <summary>Chave do RPS a consultar. Mutuamente exclusivo com <see cref="ChaveNfe"/>.</summary>
    [XmlElement("ChaveRPS", Form = XmlSchemaForm.Unqualified)]
    public ChaveRps? ChaveRps { get; set; }

    /// <summary>Chave da NFS-e a consultar. Mutuamente exclusivo com <see cref="ChaveRps"/>.</summary>
    [XmlElement("ChaveNFe", Form = XmlSchemaForm.Unqualified)]
    public ChaveNfe? ChaveNfe { get; set; }
}