using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Tipo de valor incluído neste documento, recebido por motivo de estarem relacionadas a operações de terceiros, objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "We have to abide to XML Schema (XSD) that doesn't publish zero")]
public enum TipoValorIncluso
{
    /// <summary>Repasse de remuneração por intermediação de imóveis a demais corretores envolvidos na operação.</summary>
    [XmlEnum("01")]
    RepasseIntermediacaoImoveis = 1,

    /// <summary>Repasse de valores a fornecedor relativo a fornecimento intermediado por agência de turismo.</summary>
    [XmlEnum("02")]
    RepasseFornecedorTurismo = 2,

    /// <summary>Reembolso ou ressarcimento recebido por agência de propaganda e publicidade por valores pagos relativos a serviços de produção externa por conta e ordem de terceiro.</summary>
    [XmlEnum("03")]
    ReembolsoServicoProducaoExterna = 3,

    /// <summary>Reembolso ou ressarcimento recebido por agência de propaganda e publicidade por valores pagos relativos a serviços de mídia por conta e ordem de terceiro.</summary>
    [XmlEnum("04")]
    ReembolsoServicoMidia = 4,

    /// <summary>Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro.</summary>
    [XmlEnum("99")]
    OutrosReembolsos = 99,
}