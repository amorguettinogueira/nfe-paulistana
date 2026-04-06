using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Retorno de pedido de consulta de CNPJ processado pelo webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>RetornoConsultaCNPJ_v01.xsd</c> — Elemento raiz <c>RetornoConsultaCNPJ</c>.
/// </para>
/// <para>
/// Informa quais Inscrições Municipais (CCM) estão vinculadas a um determinado CNPJ
/// e se estes CCM emitem NFS-e ou não.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "RetornoConsultaCNPJ", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class RetornoConsultaCNPJ
{
    /// <summary>Inicializa uma nova instância de <see cref="RetornoConsultaCNPJ"/>.</summary>
    public RetornoConsultaCNPJ() { }

    /// <summary>Cabeçalho com indicativo de sucesso da operação.</summary>
    [XmlElement("Cabecalho", Namespace = "")]
    public CabecalhoRetornoConsultaCNPJ? Cabecalho { get; set; }

    /// <summary>Alertas gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Alerta", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Alerta { get; set; }

    /// <summary>Erros gerados durante o processamento. Pode ser vazio.</summary>
    [XmlElement("Erro", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public EventoRetorno[]? Erro { get; set; }

    /// <summary>
    /// Detalhes das Inscrições Municipais vinculadas ao CNPJ consultado.
    /// Cada elemento contém o CCM e se emite NFS-e.
    /// </summary>
    [XmlElement("Detalhe", Namespace = "")]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML de elementos repetidos sem elemento wrapper.")]
    public DetalheRetornoConsultaCNPJ[]? Detalhe { get; set; }
}
