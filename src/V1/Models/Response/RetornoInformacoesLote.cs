using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Retorno de pedido de informações de lote processado pelo webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>RetornoInformacoesLote_v01.xsd</c> — Elemento raiz <c>RetornoInformacoesLote</c>.
/// </para>
/// <para>
/// Informa detalhes sobre o lote de RPS consultado, incluindo quantidade de notas processadas,
/// valores totais e tempo de processamento.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoInformacoesLote", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoInformacoesLote
{
    /// <summary>Inicializa uma nova instância de <see cref="RetornoInformacoesLote"/>.</summary>
    public RetornoInformacoesLote() { }

    /// <summary>Cabeçalho com indicativo de sucesso e informações do lote consultado.</summary>
    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoLote? Cabecalho { get; set; }

    /// <summary>Alertas gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    /// <summary>Erros gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }
}
