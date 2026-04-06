using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Chave de identificação única de um RPS, composta pela Inscrição Municipal
/// do prestador, a série e o número do RPS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpChaveRPS</c>.
/// Utilizado no contexto de requisição (ex.: <c>PedidoConsultaNFe</c>).
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class ChaveRps
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ChaveRps() { }

    /// <summary>Inicializa a chave com todos os campos identificadores do RPS.</summary>
    /// <param name="inscricaoPrestador">Inscrição Municipal do prestador de serviços.</param>
    /// <param name="serieRps">Série do RPS. Opcional.</param>
    /// <param name="numeroRps">Número sequencial do RPS.</param>
    public ChaveRps(InscricaoMunicipal inscricaoPrestador, SerieRps? serieRps, Numero numeroRps)
    {
        InscricaoPrestador = inscricaoPrestador;
        SerieRps = serieRps;
        NumeroRps = numeroRps;
    }

    /// <summary>Inscrição Municipal do prestador de serviços emissor do RPS.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public InscricaoMunicipal? InscricaoPrestador { get; set; }

    /// <summary>Série alfanumérica que identifica o bloco de RPS. Opcional.</summary>
    [XmlElement("SerieRPS", Form = XmlSchemaForm.Unqualified)]
    public SerieRps? SerieRps { get; set; }

    /// <summary>Número sequencial do RPS dentro da série.</summary>
    [XmlElement("NumeroRPS", Form = XmlSchemaForm.Unqualified)]
    public Numero? NumeroRps { get; set; }
}