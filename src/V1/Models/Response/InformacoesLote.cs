using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Informações sobre o lote processado pelo webservice da NF-e Paulistana (<c>tpInformacoesLote</c>).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpInformacoesLote</c>.
/// Utiliza tipos primitivos para garantir desserialização resiliente,
/// independente das regras de validação aplicadas no envio.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class InformacoesLote
{
    /// <summary>Inicializa uma nova instância de <see cref="InformacoesLote"/>.</summary>
    public InformacoesLote() { }

    /// <summary>Número do lote processado. Opcional.</summary>
    [XmlElement("NumeroLote", Namespace = "")]
    public string? NumeroLote { get; set; }

    /// <summary>Inscrição Municipal do prestador responsável pelo lote.</summary>
    [XmlElement("InscricaoPrestador", Namespace = "")]
    public string? InscricaoPrestador { get; set; }

    /// <summary>CPF ou CNPJ do remetente do lote.</summary>
    [XmlElement("CPFCNPJRemetente", Namespace = "")]
    public CpfCnpj? CpfCnpjRemetente { get; set; }

    /// <summary>Data e hora do envio do lote ao webservice.</summary>
    [XmlElement("DataEnvioLote", Namespace = "")]
    public DateTime DataEnvioLote { get; set; }

    /// <summary>Quantidade de notas processadas no lote.</summary>
    [XmlElement("QtdNotasProcessadas", Namespace = "")]
    public long QtdNotasProcessadas { get; set; }

    /// <summary>Tempo de processamento do lote em milissegundos.</summary>
    [XmlElement("TempoProcessamento", Namespace = "")]
    public long TempoProcessamento { get; set; }

    /// <summary>Valor total dos serviços das notas processadas no lote.</summary>
    [XmlElement("ValorTotalServicos", Namespace = "")]
    public decimal ValorTotalServicos { get; set; }

    /// <summary>Valor total das deduções das notas processadas. Opcional.</summary>
    [XmlElement("ValorTotalDeducoes", Namespace = "")]
    public decimal? ValorTotalDeducoes { get; set; }
}
