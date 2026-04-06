using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Response;

/// <summary>
/// Detalhe do retorno de consulta de CNPJ v02, contendo a inscrição municipal
/// vinculada e se emite NFS-e.
/// </summary>
/// <remarks>
/// Fonte: <c>RetornoConsultaCNPJ_v02.xsd</c> — Elemento inline <c>Detalhe</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class DetalheRetornoConsultaCNPJ
{
    /// <summary>Inicializa uma nova instância de <see cref="DetalheRetornoConsultaCNPJ"/>.</summary>
    public DetalheRetornoConsultaCNPJ() { }

    /// <summary>Inscrição Municipal (CCM) vinculada ao CNPJ consultado.</summary>
    [XmlElement("InscricaoMunicipal", Namespace = "")]
    public long InscricaoMunicipal { get; set; }

    /// <summary>Indica se o CCM vinculado ao CNPJ consultado emite NFS-e.</summary>
    [XmlElement("EmiteNFe", Namespace = "")]
    public bool EmiteNFe { get; set; }
}
