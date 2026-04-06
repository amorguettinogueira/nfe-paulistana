using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Motivo pelo qual o NIF não foi informado.
/// </summary>
public enum MotivoNifNaoInformado
{
    /// <summary>Não informado na nota de origem.</summary>
    [XmlEnum("0")]
    NaoInformadoNaOrigem = 0,

    /// <summary>Dispensado do NIF.</summary>
    [XmlEnum("1")]
    Dispensado = 1,

    /// <summary>Não exigência do NIF.</summary>
    [XmlEnum("2")]
    SemExigencia = 2
}