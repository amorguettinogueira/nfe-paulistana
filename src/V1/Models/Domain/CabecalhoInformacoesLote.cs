using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Cabeçalho do pedido de informações de lote (<c>PedidoInformacoesLote</c>).
/// Estende <see cref="Cabecalho"/> com o número do lote (opcional) e a inscrição
/// municipal do prestador (obrigatória).
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoInformacoesLote_v01.xsd</c> — Elemento <c>Cabecalho</c>.
/// </para>
/// <para>
/// Quando <see cref="NumeroLote"/> não é informado, o webservice retorna informações
/// do último lote gerador de NFS-e do prestador.
/// </para>
/// </remarks>
[Serializable]
public sealed class CabecalhoInformacoesLote : Cabecalho
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CabecalhoInformacoesLote()
    { }

    /// <summary>Inicializa o cabeçalho de informações de lote com o documento fiscal do remetente.</summary>
    /// <param name="value">CPF ou CNPJ do remetente.</param>
    public CabecalhoInformacoesLote(CpfOrCnpj value) => CpfOrCnpj = value;

    /// <summary>
    /// Número do lote que se deseja obter informações.
    /// Quando não informado, retorna informações do último lote gerador de NFS-e.
    /// </summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Numero? NumeroLote { get; set; }

    /// <summary>Inscrição municipal do prestador de serviços que gerou o lote.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public InscricaoMunicipal? InscricaoPrestador { get; set; }
}