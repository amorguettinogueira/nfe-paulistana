using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Representa um documento fiscal brasileiro — CPF ou CNPJ (<c>tpCPFCNPJ</c>) —
/// no contexto de resposta do webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Difere de <c>Nfe.Paulistana.Models.Domain.CpfOrCnpj</c> por usar tipos primitivos
/// em vez de Value Objects, garantindo desserialização resiliente de dados retornados
/// pelo webservice sem impor validações de envio.
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpCPFCNPJ</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CpfCnpj
{
    /// <summary>Inicializa uma nova instância de <see cref="CpfCnpj"/>.</summary>
    public CpfCnpj() { }

    /// <summary>CPF do titular. Mutuamente exclusivo com <see cref="Cnpj"/>.</summary>
    [XmlElement("CPF", Namespace = "")]
    public string? Cpf { get; set; }

    /// <summary>CNPJ do titular. Mutuamente exclusivo com <see cref="Cpf"/>.</summary>
    [XmlElement("CNPJ", Namespace = "")]
    public string? Cnpj { get; set; }

    /// <summary>Retorna a representação textual: CPF quando definido, CNPJ caso contrário.</summary>
    public override string? ToString() => Cpf ?? Cnpj;
}
