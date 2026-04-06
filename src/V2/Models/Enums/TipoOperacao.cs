using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Tipo de Operação com Entes Governamentais ou outros serviços sobre bens imóveis.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "We have to abide to XML Schema (XSD) that doesn't publish zero")]
public enum TipoOperacao
{
    /// <summary>Fornecimento com pagamento posterior.</summary>
    [XmlEnum("1")]
    PagamentoPosterior = 1,

    /// <summary>Recebimento do pagamento com fornecimento já realizado.</summary>
    [XmlEnum("2")]
    RecebimentoComPagamento = 2,

    /// <summary>Fornecimento com pagamento já realizado.</summary>
    [XmlEnum("3")]
    FornecimentoComPagamentoRealizado = 3,

    /// <summary>Recebimento do pagamento com fornecimento posterior.</summary>
    [XmlEnum("4")]
    RecebimentoComPagamentoPosterior = 4,

    /// <summary>Fornecimento e recebimento do pagamento concomitantes.</summary>
    [XmlEnum("5")]
    FornecimentoERecebimentoConcomitantes = 5,
}