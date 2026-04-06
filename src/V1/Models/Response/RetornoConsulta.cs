using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Retorno de pedido de consulta de NFS-e processado pelo webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoConsulta_v01.xsd</c> — Elemento raiz <c>RetornoConsulta</c>.
/// Compartilhado pelas operações de consulta individual de NFS-e (<c>ConsultaNFe</c>),
/// NFS-e recebidas (<c>ConsultaNFeRecebidas</c>) e lote (<c>ConsultaLote</c>).
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoConsulta", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoConsulta
{
    /// <summary>Inicializa uma nova instância de <see cref="RetornoConsulta"/>.</summary>
    public RetornoConsulta() { }

    /// <summary>Cabeçalho com indicativo de sucesso da operação.</summary>
    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoConsulta? Cabecalho { get; set; }

    /// <summary>Alertas gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    /// <summary>Erros gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }

    /// <summary>NFS-e retornadas pela consulta. Máximo de 50 entradas.</summary>
    [XmlElement("NFe", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public NfeModel[]? Nfes { get; set; }
}
