using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Response;

/// <summary>
/// Representa um evento de alerta ou erro retornado pelo webservice da NF-e Paulistana v02 (<c>tpEvento</c>).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpEvento</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class EventoRetorno
{
    /// <summary>Inicializa uma nova instância de <see cref="EventoRetorno"/>.</summary>
    public EventoRetorno() { }

    /// <summary>Código numérico do evento de alerta ou erro.</summary>
    [XmlElement("Codigo", Namespace = "")]
    public short Codigo { get; set; }

    /// <summary>Descrição textual do evento. Opcional.</summary>
    [XmlElement("Descricao", Namespace = "")]
    public string? Descricao { get; set; }

    /// <summary>Chave do RPS relacionado ao evento. Mutuamente exclusivo com <see cref="ChaveNFe"/>.</summary>
    [XmlElement("ChaveRPS", Namespace = "")]
    public ChaveRps? ChaveRPS { get; set; }

    /// <summary>Chave da NFS-e relacionada ao evento. Mutuamente exclusivo com <see cref="ChaveRPS"/>.</summary>
    [XmlElement("ChaveNFe", Namespace = "")]
    public ChaveNFe? ChaveNFe { get; set; }
}
