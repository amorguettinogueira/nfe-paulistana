using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Retorno do pedido de envio de lote de RPS processado pelo webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoEnvioLoteRPS_v01.xsd</c> — Elemento raiz <c>RetornoEnvioLoteRPS</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoEnvioLoteRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoEnvioLoteRps
{
    /// <summary>Inicializa uma nova instância de <see cref="RetornoEnvioLoteRps"/>.</summary>
    public RetornoEnvioLoteRps() { }

    /// <summary>Cabeçalho com indicativo de sucesso e informações sobre o lote processado.</summary>
    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoLote? Cabecalho { get; set; }

    /// <summary>Alertas gerados durante o processamento do lote. Pode ser vazio.</summary>
    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    /// <summary>Erros gerados durante o processamento do lote. Pode ser vazio.</summary>
    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }

    /// <summary>Associações entre NFS-e emitidas e os RPS substituídos. Máximo de 50 entradas.</summary>
    [XmlElement("ChaveNFeRPS", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public ChaveNFeRps[]? ChavesNFeRps { get; set; }
}
