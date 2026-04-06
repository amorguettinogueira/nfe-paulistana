using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Response;

/// <summary>
/// Cabeçalho do retorno de cancelamento de NFS-e v02, indicando o sucesso da operação.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoCancelamentoNFe_v02.xsd</c> — Elemento inline <c>Cabecalho</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CabecalhoRetornoCancelamento
{
    /// <summary>Inicializa uma nova instância de <see cref="CabecalhoRetornoCancelamento"/>.</summary>
    public CabecalhoRetornoCancelamento() { }

    /// <summary>Versão do schema XSD utilizado.</summary>
    [XmlAttribute("Versao")]
    public int Versao { get; set; }

    /// <summary>Indica se o cancelamento foi processado com sucesso.</summary>
    [XmlElement("Sucesso", Namespace = "")]
    public bool Sucesso { get; set; }
}
