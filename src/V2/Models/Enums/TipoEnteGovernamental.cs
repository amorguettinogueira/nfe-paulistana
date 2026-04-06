using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Tipo do ente da compra governamental.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "We have to abide to XML Schema (XSD) that doesn't publish zero")]
public enum TipoEnteGovernamental
{
    /// <summary>União.</summary>
    [XmlEnum("1")]
    Uniao = 1,

    /// <summary>Estados.</summary>
    [XmlEnum("2")]
    Estados = 2,

    /// <summary>Distrito Federal.</summary>
    [XmlEnum("3")]
    DistritoFederal = 3,

    /// <summary>Municípios.</summary>
    [XmlEnum("4")]
    Municipios = 4
}