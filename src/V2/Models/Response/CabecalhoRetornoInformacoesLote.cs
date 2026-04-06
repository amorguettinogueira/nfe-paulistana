using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Response;

/// <summary>
/// Cabeçalho do retorno de informações de lote v02, indicando o sucesso da operação
/// e as informações sobre o lote processado.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoInformacoesLote_v02.xsd</c> — Elemento inline <c>Cabecalho</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CabecalhoRetornoInformacoesLote
{
    /// <summary>Inicializa uma nova instância de <see cref="CabecalhoRetornoInformacoesLote"/>.</summary>
    public CabecalhoRetornoInformacoesLote() { }

    /// <summary>Versão do schema XSD utilizado.</summary>
    [XmlAttribute("Versao")]
    public int Versao { get; set; }

    /// <summary>Indica se o pedido de informações foi processado com sucesso.</summary>
    [XmlElement("Sucesso", Namespace = "")]
    public bool Sucesso { get; set; }

    /// <summary>Informações detalhadas sobre o lote processado. Presente apenas quando <see cref="Sucesso"/> é verdadeiro.</summary>
    [XmlElement("InformacoesLote", Namespace = "")]
    public InformacoesLote? InformacoesLote { get; set; }
}
