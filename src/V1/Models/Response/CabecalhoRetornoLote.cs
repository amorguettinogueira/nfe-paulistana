using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Cabeçalho do retorno de envio de lote de RPS, indicando o sucesso da operação
/// e as informações sobre o lote processado.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoEnvioLoteRPS_v01.xsd</c> — Elemento inline <c>Cabecalho</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CabecalhoRetornoLote
{
    /// <summary>Inicializa uma nova instância de <see cref="CabecalhoRetornoLote"/>.</summary>
    public CabecalhoRetornoLote() { }

    /// <summary>Versão do schema XSD utilizado. Valor fixo: 1.</summary>
    [XmlAttribute("Versao")]
    public int Versao { get; set; }

    /// <summary>Indica se o pedido de envio foi processado com sucesso.</summary>
    [XmlElement("Sucesso", Namespace = "")]
    public bool Sucesso { get; set; }

    /// <summary>Informações detalhadas sobre o lote processado. Presente apenas quando <see cref="Sucesso"/> é verdadeiro.</summary>
    [XmlElement("InformacoesLote", Namespace = "")]
    public InformacoesLote? InformacoesLote { get; set; }
}
