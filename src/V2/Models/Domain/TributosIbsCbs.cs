using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Informações relacionadas aos tributos IBS e à CBS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpTrib</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class TributosIbsCbs
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TributosIbsCbs() { }

    /// <summary>Inicializa a instância com a classificação tributária e a tributação regular especificadas.</summary>
    /// <param name="tributoIbsCbs">Informações relacionadas ao IBS e à CBS.</param>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="tributoIbsCbs"/> for nulo.</exception>
    public TributosIbsCbs(TributoIbsCbs tributoIbsCbs)
    {
        ArgumentNullException.ThrowIfNull(tributoIbsCbs, nameof(tributoIbsCbs));

        TributoIbsCbs = tributoIbsCbs;
    }

    /// <summary>Informações relacionadas ao IBS e à CBS.</summary>
    [XmlElement("gIBSCBS", Form = XmlSchemaForm.Unqualified)]
    public TributoIbsCbs? TributoIbsCbs { get; set; }
}