using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Indicador da finalidade da emissão de NFS-e.
/// </summary>
public enum FinalidadeEmissaoNfe
{
    /// <summary>NFS-e regular.</summary>
    [XmlEnum("0")]
    NfseRegular = 0,
}