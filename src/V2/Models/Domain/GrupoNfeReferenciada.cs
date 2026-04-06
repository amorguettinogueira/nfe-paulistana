using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Grupo com Ids da nota nacional referenciadas, associadas a NFSE.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpGRefNFSe</c>.
/// Utilizado no contexto de requisição (ex.: <c>PedidoConsultaNFe</c>).
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class GrupoNfeReferenciada
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public GrupoNfeReferenciada() { }

    /// <summary>Inicializa a chave com todas as notas nacionais referenciadas.</summary>
    /// <param name="nfeReferenciadas">Coleção de notas nacionais referenciadas.</param>
    public GrupoNfeReferenciada(IEnumerable<ChaveNotaNacional> nfeReferenciadas)
    {
        ArgumentNullException.ThrowIfNull(nfeReferenciadas, nameof(nfeReferenciadas));

        if (!(nfeReferenciadas?.Any() ?? false))
        {
            throw new ArgumentException("A coleção de notas nacionais referenciadas deve conter ao menos um item.", nameof(nfeReferenciadas));
        }

        if ((nfeReferenciadas?.Count() ?? 0) > 99)
        {
            throw new ArgumentException("A coleção de notas nacionais referenciadas não pode conter mais de 99 itens.", nameof(nfeReferenciadas));
        }

        NfeReferenciadas = [.. nfeReferenciadas!];
    }

    /// <summary>Grupo com Ids da nota nacional referenciadas, associadas a NFSE.</summary>
    [XmlElement("refNFSe", Form = XmlSchemaForm.Unqualified)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML.")]
    public ChaveNotaNacional[]? NfeReferenciadas { get; set; }
}