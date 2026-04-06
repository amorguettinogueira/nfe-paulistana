using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Response;

/// <summary>
/// Representa a chave de identificação de uma NFS-e (<c>tpChaveNFe</c>) no contexto de
/// resposta do webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpChaveNFe</c>.
/// Utiliza tipos primitivos para garantir desserialização resiliente,
/// independente das regras de validação aplicadas no envio.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = "")]
[Serializable]
public sealed class ChaveNFe
{
    /// <summary>Inicializa uma nova instância de <see cref="ChaveNFe"/>.</summary>
    public ChaveNFe() { }

    /// <summary>Inscrição Municipal do prestador emissor da NFS-e.</summary>
    [XmlElement("InscricaoPrestador", Namespace = "")]
    public string? InscricaoPrestador { get; set; }

    /// <summary>Número sequencial da NFS-e emitida.</summary>
    [XmlElement("NumeroNFe", Namespace = "")]
    public string? NumeroNFe { get; set; }

    /// <summary>Código de verificação da NFS-e. Opcional.</summary>
    [XmlElement("CodigoVerificacao", Namespace = "")]
    public string? CodigoVerificacao { get; set; }
}
