using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Endereço retornado pelo webservice da NF-e Paulistana no contexto de resposta (<c>tpEndereco</c>).
/// </summary>
/// <remarks>
/// Difere de <c>Nfe.Paulistana.Models.Domain.Endereco</c> por usar tipos primitivos
/// em vez de Value Objects, garantindo desserialização resiliente de dados retornados
/// pelo webservice sem impor validações de envio.
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpEndereco</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class EnderecoNfe
{
    /// <summary>Inicializa uma nova instância de <see cref="EnderecoNfe"/>.</summary>
    public EnderecoNfe() { }

    /// <summary>Tipo do logradouro. Opcional.</summary>
    [XmlElement("TipoLogradouro", Namespace = "")]
    public string? TipoLogradouro { get; set; }

    /// <summary>Nome do logradouro. Opcional.</summary>
    [XmlElement("Logradouro", Namespace = "")]
    public string? Logradouro { get; set; }

    /// <summary>Número do endereço. Opcional.</summary>
    [XmlElement("NumeroEndereco", Namespace = "")]
    public string? NumeroEndereco { get; set; }

    /// <summary>Complemento do endereço. Opcional.</summary>
    [XmlElement("ComplementoEndereco", Namespace = "")]
    public string? ComplementoEndereco { get; set; }

    /// <summary>Bairro. Opcional.</summary>
    [XmlElement("Bairro", Namespace = "")]
    public string? Bairro { get; set; }

    /// <summary>Código do município (IBGE). Opcional.</summary>
    [XmlElement("Cidade", Namespace = "")]
    public string? Cidade { get; set; }

    /// <summary>Unidade federativa (UF). Opcional.</summary>
    [XmlElement("UF", Namespace = "")]
    public string? Uf { get; set; }

    /// <summary>Código de Endereçamento Postal (CEP). Opcional.</summary>
    [XmlElement("CEP", Namespace = "")]
    public string? Cep { get; set; }
}
