using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Par de chaves que associa uma NFS-e ao RPS que a originou (<c>tpChaveNFeRPS</c>).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpChaveNFeRPS</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class ChaveNFeRps
{
    /// <summary>Inicializa uma nova instância de <see cref="ChaveNFeRps"/>.</summary>
    public ChaveNFeRps() { }

    /// <summary>Chave de identificação da NFS-e emitida.</summary>
    [XmlElement("ChaveNFe", Namespace = "")]
    public ChaveNFe? ChaveNFe { get; set; }

    /// <summary>Chave de identificação do RPS substituído pela NFS-e.</summary>
    [XmlElement("ChaveRPS", Namespace = "")]
    public ChaveRps? ChaveRPS { get; set; }
}
