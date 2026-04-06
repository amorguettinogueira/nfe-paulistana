using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Cabeçalho do retorno de envio de RPS unitário, indicando o sucesso da operação.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoEnvioRPS_v01.xsd</c> — Elemento inline <c>Cabecalho</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CabecalhoRetornoRps
{
    /// <summary>Inicializa uma nova instância de <see cref="CabecalhoRetornoRps"/>.</summary>
    public CabecalhoRetornoRps() { }

    /// <summary>Versão do schema XSD utilizado. Valor fixo: 1.</summary>
    [XmlAttribute("Versao")]
    public int Versao { get; set; }

    /// <summary>Indica se o pedido de envio foi processado com sucesso.</summary>
    [XmlElement("Sucesso", Namespace = "")]
    public bool Sucesso { get; set; }
}
