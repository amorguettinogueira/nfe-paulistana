using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Cabeçalho do pedido de consulta de lote (<c>PedidoConsultaLote</c>).
/// Estende <see cref="Cabecalho"/> com o número do lote a consultar.
/// </summary>
/// <remarks>
/// Fonte: <c>PedidoConsultaLote_v01.xsd</c> — Elemento <c>Cabecalho</c>.
/// </remarks>
[Serializable]
public sealed class CabecalhoConsultaLote : Cabecalho
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CabecalhoConsultaLote()
    { }

    /// <summary>Inicializa o cabeçalho de consulta de lote com o documento fiscal do remetente.</summary>
    /// <param name="value">CPF ou CNPJ do remetente.</param>
    public CabecalhoConsultaLote(CpfOrCnpj value) => CpfOrCnpj = value;

    /// <summary>Número do lote que se deseja consultar.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Numero? NumeroLote { get; set; }
}