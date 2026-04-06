using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Grupo de informações de documento não fiscal.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpDocOutro</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class DocumentoNaoFiscal
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DocumentoNaoFiscal() { }

    /// <summary>Inicializa a chave com os campos identificadores do documento não fiscal.</summary>
    /// <param name="numeroDocumento">Número do documento não fiscal.</param>
    /// <param name="descricaoDocumento">Descrição do documento não fiscal.</param>
    /// <exception cref="ArgumentNullException">Lançada quando algum dos parâmetros for nulo.</exception>
    public DocumentoNaoFiscal(NumeroDescricaoDocumento numeroDocumento, NumeroDescricaoDocumento descricaoDocumento)
    {
        ArgumentNullException.ThrowIfNull(numeroDocumento, nameof(numeroDocumento));
        ArgumentNullException.ThrowIfNull(descricaoDocumento, nameof(descricaoDocumento));

        NumeroDocumento = numeroDocumento;
        DescricaoDocumento = descricaoDocumento;
    }

    /// <summary>Número do documento não fiscal.</summary>
    [XmlElement("nDoc", Form = XmlSchemaForm.Unqualified)]
    public NumeroDescricaoDocumento? NumeroDocumento { get; set; }

    /// <summary>Descrição do documento não fiscal.</summary>
    [XmlElement("xDoc", Form = XmlSchemaForm.Unqualified)]
    public NumeroDescricaoDocumento? DescricaoDocumento { get; set; }
}