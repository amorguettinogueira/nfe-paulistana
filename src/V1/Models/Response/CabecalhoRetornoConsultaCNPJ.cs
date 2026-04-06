using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Cabeçalho do retorno de consulta de CNPJ, indicando o sucesso da operação.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoConsultaCNPJ_v01.xsd</c> — Elemento inline <c>Cabecalho</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class CabecalhoRetornoConsultaCNPJ
{
    /// <summary>Inicializa uma nova instância de <see cref="CabecalhoRetornoConsultaCNPJ"/>.</summary>
    public CabecalhoRetornoConsultaCNPJ() { }

    /// <summary>Versão do schema XSD utilizado. Valor fixo: 1.</summary>
    [XmlAttribute("Versao")]
    public int Versao { get; set; }

    /// <summary>Indica se a consulta de CNPJ foi processada com sucesso.</summary>
    [XmlElement("Sucesso", Namespace = "")]
    public bool Sucesso { get; set; }
}
