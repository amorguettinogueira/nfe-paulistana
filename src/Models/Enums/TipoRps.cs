using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Models.Enums;

// Define o tipo de RPS.
// Fonte: TiposNFe_v01.xsd
// Tipo: tpTipoRPS
// Linha: 302
/// <summary>
/// Tipo de RPS: RPS, Nota Fiscal Conjugada (Mista) ou Cupom
/// </summary>
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "We have to abide to XML Schema (XSD) that doesn't publish zero")]
public enum TipoRps
{
    /// <summary>Recibo Provisório de Serviços padrão.</summary>
    [XmlEnum("RPS")]
    Rps = 1,

    /// <summary>Nota Fiscal Conjugada (Mista), que agrupa prestação de serviços e fornecimento de mercadorias.</summary>
    [XmlEnum("RPS-M")]
    NotaFiscalConjugada = 2,

    /// <summary>Cupom fiscal emitido no varejo de serviços.</summary>
    [XmlEnum("RPS-C")]
    Cupom = 3
}