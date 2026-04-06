using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Retorno de pedido de cancelamento de NFS-e processado pelo webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoCancelamentoNFe_v01.xsd</c> — Elemento raiz <c>RetornoCancelamentoNFe</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoCancelamentoNFe", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoCancelamentoNFe
{
    /// <summary>Inicializa uma nova instância de <see cref="RetornoCancelamentoNFe"/>.</summary>
    public RetornoCancelamentoNFe() { }

    /// <summary>Cabeçalho com indicativo de sucesso da operação.</summary>
    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoCancelamento? Cabecalho { get; set; }

    /// <summary>Alertas gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    /// <summary>Erros gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }
}
