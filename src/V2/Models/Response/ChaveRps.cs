using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Response;

/// <summary>
/// Representa a chave de identificação de um RPS (<c>tpChaveRPS</c>) no contexto de
/// resposta do webservice da NF-e Paulistana v02.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpChaveRPS</c>.
/// Utiliza tipos primitivos para garantir desserialização resiliente,
/// independente das regras de validação aplicadas no envio.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class ChaveRps
{
    /// <summary>Inicializa uma nova instância de <see cref="ChaveRps"/>.</summary>
    public ChaveRps() { }

    /// <summary>Inscrição Municipal do prestador emissor do RPS.</summary>
    [XmlElement("InscricaoPrestador", Namespace = "")]
    public string? InscricaoPrestador { get; set; }

    /// <summary>Série alfanumérica do RPS. Opcional.</summary>
    [XmlElement("SerieRPS", Namespace = "")]
    public string? SerieRps { get; set; }

    /// <summary>Número sequencial do RPS dentro da série.</summary>
    [XmlElement("NumeroRPS", Namespace = "")]
    public string? NumeroRps { get; set; }
}
