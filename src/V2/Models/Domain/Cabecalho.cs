using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Cabeçalho do envelope de envio da NF-e Paulistana v02,
/// contendo o CPF ou CNPJ do remetente e o número de versão do schema.
/// </summary>
/// <remarks>
/// Fonte: <c>PedidoConsultaCNPJ_v02.xsd</c> — Elemento <c>Cabecalho</c>.
/// A versão do schema é sempre <c>2</c> para a v02.
/// </remarks>
[Serializable]
public class Cabecalho
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Cabecalho()
    { }

    /// <summary>Inicializa o cabeçalho com o documento fiscal do remetente.</summary>
    /// <param name="value">CPF ou CNPJ do remetente.</param>
    public Cabecalho(CpfOrCnpj value) => CpfOrCnpj = value;

    /// <summary>CPF ou CNPJ do remetente da solicitação.</summary>
    [XmlElement("CPFCNPJRemetente", Form = XmlSchemaForm.Unqualified)]
    public CpfOrCnpj? CpfOrCnpj { get; set; }

    /// <summary>Versão do schema XML. Valor fixo: <c>2</c>.</summary>
    [XmlAttribute()]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static members are not serialized to XML")]
    public int Versao { get; set; } = 2;

    /// <summary>Cria um cabeçalho a partir de um CPF.</summary>
    /// <param name="value">CPF do remetente.</param>
    /// <returns>Nova instância de <see cref="Cabecalho"/> com o CPF definido.</returns>
    public static Cabecalho FromCpf(Cpf value) => new((CpfOrCnpj)value);

    /// <summary>Cria um cabeçalho a partir de um CNPJ alfanumérico.</summary>
    /// <param name="value">CNPJ do remetente.</param>
    /// <returns>Nova instância de <see cref="Cabecalho"/> com o CNPJ definido.</returns>
    public static Cabecalho FromCnpj(Cnpj value) => new((CpfOrCnpj)value);

    /// <inheritdoc cref="FromCpf"/>
    public static explicit operator Cabecalho(Cpf value) => FromCpf(value);

    /// <inheritdoc cref="FromCnpj"/>
    public static explicit operator Cabecalho(Cnpj value) => FromCnpj(value);
}