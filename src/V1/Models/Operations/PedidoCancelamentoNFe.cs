using Nfe.Paulistana.Models;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Operations;

/// <summary>
/// Define o corpo da requisição de cancelamento de NFS-e (<c>PedidoCancelamentoNFe</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoCancelamentoNFe_v01.xsd</c> — Elemento <c>PedidoCancelamentoNFe</c>.
/// </para>
/// <para>
/// Representa uma solicitação de cancelamento de até 50 NFS-e à Prefeitura de São Paulo.
/// Instâncias devem ser criadas via
/// <see cref="Builders.PedidoCancelamentoNFeFactory"/>, que garante
/// que o pedido esteja corretamente assinado antes do envio.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoCancelamentoNFe", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoCancelamentoNFe : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV1(Constants.Uris.Nfe, "PedidoCancelamentoNFe_v01.xsd");

    /// <summary>Cabeçalho da requisição contendo o CPF ou CNPJ do remetente e indicador de transação.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CabecalhoCancelamento? Cabecalho { get; set; }

    /// <summary>
    /// Detalhes do pedido, cada um contendo a chave de uma NFS-e e a assinatura de cancelamento.
    /// Mínimo 1, máximo 50 ocorrências.
    /// </summary>
    [XmlElement(ElementName = "Detalhe", Form = XmlSchemaForm.Unqualified)]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public DetalheCancelamentoNFe[]? Detalhe { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}