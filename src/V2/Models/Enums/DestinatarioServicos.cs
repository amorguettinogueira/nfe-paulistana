using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Enums;

/// <summary>
/// Indica o Destinatário dos serviços.
/// </summary>
public enum DestinatarioServicos
{
    /// <summary>O destinatário é o próprio tomador/adquirente identificado na NFS-e (tomador = adquirente = destinatário).</summary>
    [XmlEnum("0")]
    ProprioTomador = 0,

    /// <summary>O destinatário não é o próprio adquirente, podendo ser outra pessoa, física ou jurídica (ou equiparada), ou um estabelecimento diferente do indicado como tomador (tomador = adquirente ≠ destinatário).</summary>
    [XmlEnum("1")]
    OutraPessoa = 1,
}