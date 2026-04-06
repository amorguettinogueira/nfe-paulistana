using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Informações relacionadas ao IBS e à CBS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpGIBSCBS</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class TributoIbsCbs
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TributoIbsCbs() { }

    /// <summary>Inicializa a instância com a classificação tributária e a tributação regular especificadas.</summary>
    /// <param name="classificacaoTributaria">Classificação tributária.</param>
    /// <param name="tributoRegular">Informações relacionadas à tributação regular.</param>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="classificacaoTributaria"/> for nulo.</exception>"
    public TributoIbsCbs(ClassificacaoTributaria classificacaoTributaria, TributoRegular? tributoRegular = null)
    {
        ArgumentNullException.ThrowIfNull(classificacaoTributaria, nameof(classificacaoTributaria));

        ClassificacaoTributaria = classificacaoTributaria;
        TributoRegular = tributoRegular;
    }

    /// <summary>Código de classificação Tributária do IBS e da CBS.</summary>
    [XmlElement("cClassTrib", Form = XmlSchemaForm.Unqualified)]
    public ClassificacaoTributaria? ClassificacaoTributaria { get; set; }

    /// <summary>Informações relacionadas à tributação regular.</summary>
    [XmlElement("gTribRegular", Form = XmlSchemaForm.Unqualified)]
    public TributoRegular? TributoRegular { get; set; }
}