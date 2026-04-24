using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Models.Enums;

// Define o status de uma NF-e.
// Fonte: TiposNFe_v01.xsd
// Tipo: tpStatusNFe
// Linha: 255
/// <summary>
/// Status da NF-e: Normal, Cancelada ou Extraviada
/// </summary>
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Propriedade necessária para compatibilidade com XML Schema (XSD)")]
public enum StatusNfe
{
    /// <summary>NF-e em situação normal, com validade fiscal plena.</summary>
    [XmlEnum("N")]
    Normal = 1,

    /// <summary>NF-e cancelada pelo emitente após sua emissão.</summary>
    [XmlEnum("C")]
    Cancelada = 2,

    /// <summary>NF-e declarada extraviada pelo emitente.</summary>
    [XmlEnum("E")]
    Extraviada = 3
}