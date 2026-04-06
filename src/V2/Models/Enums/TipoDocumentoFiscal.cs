using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Tipo de Documento fiscal a que se refere a chaveDfe que seja um dos documentos do Repositório Nacional.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "We have to abide to XML Schema (XSD) that doesn't publish zero")]
public enum TipoDocumentoFiscal
{
    /// <summary>NFS-e.</summary>
    [XmlEnum("1")]
    Nfse = 1,

    /// <summary>NF-e.</summary>
    [XmlEnum("2")]
    Nfe = 2,

    /// <summary>CT-e.</summary>
    [XmlEnum("3")]
    Cte = 3,

    /// <summary>Outro.</summary>
    [XmlEnum("9")]
    Outro = 9,
}