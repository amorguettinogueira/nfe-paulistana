using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Cabeçalho do retorno de consulta de NFS-e, indicando o sucesso da operação.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoConsulta_v01.xsd</c> — Elemento inline <c>Cabecalho</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CabecalhoRetornoConsulta
{
    /// <summary>Inicializa uma nova instância de <see cref="CabecalhoRetornoConsulta"/>.</summary>
    public CabecalhoRetornoConsulta() { }

    /// <summary>Versão do schema XSD utilizado. Valor fixo: 1.</summary>
    [XmlAttribute("Versao")]
    public int Versao { get; set; }

    /// <summary>Indica se a consulta foi processada com sucesso.</summary>
    [XmlElement("Sucesso", Namespace = "")]
    public bool Sucesso { get; set; }
}
