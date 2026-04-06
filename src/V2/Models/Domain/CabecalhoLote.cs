using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Cabeçalho do envelope de envio em lote de RPS (<c>PedidoEnvioLoteRPS</c>) para v02.
/// Estende <see cref="Cabecalho"/> com campos específicos de lote: período, quantidade e totais.
/// </summary>
[Serializable]
public sealed class CabecalhoLote : Cabecalho
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CabecalhoLote()
    { }

    /// <summary>Inicializa o cabeçalho de lote com o documento fiscal do remetente.</summary>
    /// <param name="value">CPF ou CNPJ do remetente.</param>
    public CabecalhoLote(CpfOrCnpj value) : base(value) { }

    /// <summary>
    /// Indica se o envio do lote deve ser tratado como transação atômica.
    /// Padrão: <c>true</c>.
    /// </summary>
    [DefaultValue(true)]
    [XmlElement(ElementName = "transacao", Form = XmlSchemaForm.Unqualified)]
    public bool? Transacao { get; set; } = true;

    /// <summary>Data de início do período de competência do lote.</summary>
    [XmlElement(ElementName = "dtInicio", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DtInicio { get; set; }

    /// <summary>Data de fim do período de competência do lote.</summary>
    [XmlElement(ElementName = "dtFim", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DtFim { get; set; }

    /// <summary>Quantidade total de RPS contidos no lote.</summary>
    [XmlElement(ElementName = "QtdRPS", Form = XmlSchemaForm.Unqualified)]
    public Quantidade? QtdRps { get; set; }
}