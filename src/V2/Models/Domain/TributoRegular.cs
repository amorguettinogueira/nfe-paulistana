using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Informações relacionadas à tributação regular.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpGTribRegular</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class TributoRegular
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TributoRegular() { }

    /// <summary>Inicializa uma instância com a classificação tributária especificada.</summary>
    /// <param name="classificacaoTributaria">Classificação tributária.</param>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="classificacaoTributaria"/> for nulo.</exception>
    public TributoRegular(ClassificacaoTributaria classificacaoTributaria)
    {
        ArgumentNullException.ThrowIfNull(classificacaoTributaria, nameof(classificacaoTributaria));

        ClassificacaoTributaria = classificacaoTributaria;
    }

    /// <summary>Código de classificação Tributária do IBS e da CBS.</summary>
    [XmlElement("cClassTribReg", Form = XmlSchemaForm.Unqualified)]
    public ClassificacaoTributaria? ClassificacaoTributaria { get; set; }
}