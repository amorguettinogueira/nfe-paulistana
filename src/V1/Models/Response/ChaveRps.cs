using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Representa a chave de identificação de um RPS (<c>tpChaveRPS</c>) no contexto de
/// resposta do webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Difere de <c>Nfe.Paulistana.Models.Domain.ChaveRps</c> por usar tipos primitivos
/// em vez de Value Objects, garantindo desserialização resiliente de dados retornados
/// pelo webservice sem impor validações de envio.
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpChaveRPS</c>.
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
