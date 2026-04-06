using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Cabeçalho do pedido de consulta de NFS-e por período v02 (<c>PedidoConsultaNFePeriodo</c>).
/// Estende <see cref="Cabecalho"/> com os campos de filtro por CPF/CNPJ do
/// prestador ou tomador, inscrição municipal, período de datas e paginação.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoConsultaNFePeriodo_v02.xsd</c> — Elemento <c>Cabecalho</c>.
/// </para>
/// <para>
/// Compartilhado pelas operações <c>ConsultaNFeRecebidas</c> (filtra pelo tomador)
/// e <c>ConsultaNFeEmitidas</c> (filtra pelo prestador) da v02.
/// </para>
/// </remarks>
[Serializable]
public sealed class CabecalhoConsultaPeriodo : Cabecalho
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CabecalhoConsultaPeriodo()
    { }

    /// <summary>Inicializa o cabeçalho de consulta por período com o documento fiscal do remetente.</summary>
    /// <param name="value">CPF ou CNPJ do remetente.</param>
    public CabecalhoConsultaPeriodo(CpfOrCnpj value) => CpfOrCnpj = value;

    /// <summary>
    /// CPF ou CNPJ do prestador (emitidas) ou tomador (recebidas) a consultar.
    /// </summary>
    [XmlElement("CPFCNPJ", Form = XmlSchemaForm.Unqualified)]
    public CpfOrCnpj? CpfCnpj { get; set; }

    /// <summary>
    /// Inscrição Municipal do prestador (emitidas) ou tomador (recebidas).
    /// Obrigatório para consulta de NFS-e emitidas; opcional para recebidas.
    /// </summary>
    [XmlElement("Inscricao", Form = XmlSchemaForm.Unqualified)]
    public InscricaoMunicipal? Inscricao { get; set; }

    /// <summary>Data de início do período a consultar.</summary>
    [XmlElement(ElementName = "dtInicio", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DtInicio { get; set; }

    /// <summary>Data de fim do período a consultar.</summary>
    [XmlElement(ElementName = "dtFim", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DtFim { get; set; }

    /// <summary>
    /// Número da página de resultados a consultar. Padrão: <c>1</c>.
    /// </summary>
    [DefaultValue(1)]
    [XmlElement(ElementName = "NumeroPagina", Form = XmlSchemaForm.Unqualified)]
    public Numero? NumeroPagina { get; set; }
}