using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Cabeçalho do pedido de cancelamento de NFS-e (<c>PedidoCancelamentoNFe</c>).
/// Estende <see cref="Cabecalho"/> com o campo <c>transacao</c>, indicando se os
/// cancelamentos devem ser tratados como transação atômica.
/// </summary>
/// <remarks>
/// Fonte: <c>PedidoCancelamentoNFe_v01.xsd</c> — Elemento <c>Cabecalho</c>.
/// </remarks>
[Serializable]
public sealed class CabecalhoCancelamento : Cabecalho
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CabecalhoCancelamento()
    { }

    /// <summary>Inicializa o cabeçalho de cancelamento com o documento fiscal do remetente.</summary>
    /// <param name="value">CPF ou CNPJ do remetente.</param>
    public CabecalhoCancelamento(CpfOrCnpj value) => CpfOrCnpj = value;

    /// <summary>
    /// Indica se os cancelamentos devem ser tratados como transação atômica.
    /// Padrão: <c>true</c>.
    /// </summary>
    [DefaultValue(true)]
    [XmlElement(ElementName = "transacao", Form = XmlSchemaForm.Unqualified)]
    public bool? Transacao { get; set; } = true;
}