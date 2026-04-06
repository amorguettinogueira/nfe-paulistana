using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Informações relacionadas aos valores do serviço prestado para IBS e à CBS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpValores</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class Valores
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Valores() { }

    /// <summary>Inicializa a instância com os tributos IBS e CBS e os valores inclusos especificados.</summary>
    /// <param name="tributosIbsCbs">Informações relacionadas aos tributos IBS e CBS.</param>
    /// <param name="grupoValorIncluso">Grupo de valores inclusos. Opcional.</param>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="tributosIbsCbs"/> for nulo.</exception>
    public Valores(TributosIbsCbs tributosIbsCbs, GrupoValorIncluso? grupoValorIncluso = null)
    {
        ArgumentNullException.ThrowIfNull(tributosIbsCbs, nameof(tributosIbsCbs));

        TributosIbsCbs = tributosIbsCbs;
        GrupoValoresInclusos = grupoValorIncluso;
    }

    /// <summary>Grupo de informações relativas a valores incluídos neste documento e recebidos por motivo de estarem relacionadas a operações de terceiros, objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciados.</summary>
    [XmlElement("gReeRepRes", Form = XmlSchemaForm.Unqualified)]
    public GrupoValorIncluso? GrupoValoresInclusos { get; set; }

    /// <summary>Grupo de informações relacionados aos tributos IBS e CBS.</summary>
    [XmlElement("trib", Form = XmlSchemaForm.Unqualified)]
    public TributosIbsCbs? TributosIbsCbs { get; set; }
}