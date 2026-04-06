using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Tipo de informações relativas à atividades de eventos.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpAtividadeEvento</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class AtividadeEvento
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AtividadeEvento() { }

    /// <summary>Inicializa a chave com .</summary>
    /// <param name="nomeEvento">Nome do evento cultural, artístico, esportivo.</param>
    /// <param name="dataInicioEvento">Data de início do evento.</param>
    /// <param name="dataFimEvento">Data de término do evento.</param>
    /// <param name="enderecoEvento">Endereço do evento.</param>
    /// <exception cref="ArgumentNullException">Lançada quando <paramref name="nomeEvento"/>, <paramref name="dataInicioEvento"/>, <paramref name="dataFimEvento"/> ou <paramref name="enderecoEvento"/> é nulo.</exception>
    public AtividadeEvento(
        NomeEvento? nomeEvento = null,
        DataXsd? dataInicioEvento = null,
        DataXsd? dataFimEvento = null,
        EnderecoSimplesIBSCBS? enderecoEvento = null)
    {
        ArgumentNullException.ThrowIfNull(nomeEvento, nameof(nomeEvento));
        ArgumentNullException.ThrowIfNull(dataInicioEvento, nameof(dataInicioEvento));
        ArgumentNullException.ThrowIfNull(dataFimEvento, nameof(dataFimEvento));
        ArgumentNullException.ThrowIfNull(enderecoEvento, nameof(enderecoEvento));

        NomeEvento = nomeEvento;
        DataInicioEvento = dataInicioEvento;
        DataFimEvento = dataFimEvento;
        EnderecoEvento = enderecoEvento;
    }

    /// <summary>Nome do evento cultural, artístico, esportivo.</summary>
    [XmlElement("xNomeEvt", Form = XmlSchemaForm.Unqualified)]
    public NomeEvento? NomeEvento { get; set; }

    /// <summary>Data de início da atividade de evento. Ano, Mês e Dia (AAAA-MM-DD).</summary>
    [XmlElement("dtIniEvt", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DataInicioEvento { get; set; }

    /// <summary>Data de término da atividade de evento. Ano, Mês e Dia (AAAA-MM-DD).</summary>
    [XmlElement("dtFimEvt", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DataFimEvento { get; set; }

    /// <summary>Endereço do Evento.</summary>
    [XmlElement("end", Form = XmlSchemaForm.Unqualified)]
    public EnderecoSimplesIBSCBS? EnderecoEvento { get; set; }
}