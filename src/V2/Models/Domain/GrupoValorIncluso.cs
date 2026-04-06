using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Grupo de informações relativas a valores incluídos neste documento e recebidos por
/// motivo de estarem relacionadas a operações de terceiros, objeto de reembolso, repasse ou
/// ressarcimento pelo recebedor, já tributados e aqui referenciados.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpGrupoReeRepRes</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class GrupoValorIncluso
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public GrupoValorIncluso() { }

    /// <summary>Inicializa a chave com todas as notas nacionais referenciadas.</summary>
    /// <param name="documentos">Coleção de notas nacionais referenciadas.</param>
    public GrupoValorIncluso(IEnumerable<Documento> documentos)
    {
        ArgumentNullException.ThrowIfNull(documentos, nameof(documentos));

        if (!(documentos?.Any() ?? false))
        {
            throw new ArgumentException("A coleção de documentos deve conter ao menos um item.", nameof(documentos));
        }

        if ((documentos?.Count() ?? 0) > 100)
        {
            throw new ArgumentException("A coleção de documentos não pode conter mais de 100 itens.", nameof(documentos));
        }

        Documentos = [.. documentos!];
    }

    /// <summary>Grupo de documentos referenciados nos casos de reembolso, repasse e ressarcimento que serão considerados na base de cálculo do ISSQN, do IBS e da CBS.</summary>
    [XmlElement("documentos", Form = XmlSchemaForm.Unqualified)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML.")]
    public Documento[]? Documentos { get; set; }
}