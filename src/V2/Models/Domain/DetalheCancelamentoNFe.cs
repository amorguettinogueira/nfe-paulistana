using Nfe.Paulistana.Models;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Detalhe do pedido de cancelamento de NFS-e v02, contendo a chave de identificação
/// da NFS-e e a assinatura de cancelamento. Representa o elemento inline <c>Detalhe</c>
/// de <c>PedidoCancelamentoNFe</c>.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoCancelamentoNFe_v02.xsd</c> — Elemento <c>Detalhe</c> (1–50 ocorrências).
/// </para>
/// <para>
/// A propriedade <see cref="AssinaturaCancelamento"/> armazena a assinatura RSA-SHA1
/// gerada a partir do texto de cancelamento (InscricaoPrestador + NumeroNFe),
/// serializada como <c>base64Binary</c> no XML.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[Serializable]
public sealed class DetalheCancelamentoNFe : ISignedElement
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DetalheCancelamentoNFe() { }

    /// <summary>Inicializa o detalhe com a chave da NFS-e a ser cancelada.</summary>
    /// <param name="chaveNfe">Chave da NFS-e a cancelar.</param>
    public DetalheCancelamentoNFe(ChaveNfe chaveNfe) => ChaveNfe = chaveNfe;

    /// <summary>Chave da NFS-e a ser cancelada.</summary>
    [XmlElement("ChaveNFe", Form = XmlSchemaForm.Unqualified)]
    public ChaveNfe? ChaveNfe { get; set; }

    /// <summary>
    /// Assinatura digital de cancelamento da NFS-e (<c>tpAssinaturaCancelamento</c>).
    /// Armazena os bytes da assinatura RSA-SHA1, serializados como <c>base64Binary</c>.
    /// </summary>
    [XmlElement("AssinaturaCancelamento", Form = XmlSchemaForm.Unqualified)]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This is only used during XML signing and goal is to be serialized as a base64")]
    public byte[]? AssinaturaCancelamento { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    byte[]? ISignedElement.Assinatura
    {
        get => AssinaturaCancelamento;
        set => AssinaturaCancelamento = value;
    }
}