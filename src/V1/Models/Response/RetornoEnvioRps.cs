using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Retorno do pedido de envio de RPS unitário processado pelo webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoEnvioRPS_v01.xsd</c> — Elemento raiz <c>RetornoEnvioRPS</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoEnvioRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoEnvioRps
{
    /// <summary>Inicializa uma nova instância de <see cref="RetornoEnvioRps"/>.</summary>
    public RetornoEnvioRps() { }

    /// <summary>Cabeçalho com indicativo de sucesso da operação.</summary>
    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoRps? Cabecalho { get; set; }

    /// <summary>Alertas gerados durante o processamento do RPS. Pode ser vazio.</summary>
    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    /// <summary>Erros gerados durante o processamento do RPS. Pode ser vazio.</summary>
    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }

    /// <summary>Associação entre a NFS-e emitida e o RPS substituído. Presente apenas quando o processamento foi bem-sucedido.</summary>
    [XmlElement("ChaveNFeRPS", Namespace = "")]
    public ChaveNFeRps? ChaveNFeRps { get; set; }
}
