using Nfe.Paulistana.Models;
using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V1.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Domain;

/// <summary>
/// Tipo union que representa um documento fiscal brasileiro — CPF ou CNPJ —
/// como exigido pelo XSD da NF-e Paulistana (elemento <c>tpCPFCNPJ</c>).
/// Apenas um dos campos deve estar preenchido por instância.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpCPFCNPJ</c>, linha 414.
/// </remarks>
[Serializable]
public sealed class CpfOrCnpj : ICpfOrCnpj
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CpfOrCnpj()
    { }

    /// <summary>Inicializa com um CPF.</summary>
    /// <param name="cpf">CPF a ser armazenado.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> for nulo.</exception>
    public CpfOrCnpj(Cpf cpf) =>
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));

    /// <summary>Inicializa com um CNPJ.</summary>
    /// <param name="cnpj">CNPJ a ser armazenado.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> for nulo.</exception>
    public CpfOrCnpj(Cnpj cnpj) =>
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));

    /// <summary>CPF do titular. Mutuamente exclusivo com <see cref="Cnpj"/>.</summary>
    [XmlElement("CPF", Form = XmlSchemaForm.Unqualified)]
    public Cpf? Cpf { get; set; }

    /// <summary>CNPJ do titular. Mutuamente exclusivo com <see cref="Cpf"/>.</summary>
    [XmlElement("CNPJ", Form = XmlSchemaForm.Unqualified)]
    public Cnpj? Cnpj { get; set; }

    /// <summary>
    /// Retorna a representação textual do documento: CPF quando definido, CNPJ caso contrário.
    /// </summary>
    public override string? ToString() =>
        Cpf?.ToString() ?? Cnpj?.ToString();

    /// <summary>Cria uma instância a partir de um CPF.</summary>
    /// <param name="value">CPF do titular.</param>
    /// <returns>Nova instância de <see cref="CpfOrCnpj"/> com o CPF definido.</returns>
    public static CpfOrCnpj FromCpf(Cpf value) => new(value);

    /// <summary>Cria uma instância a partir de um CNPJ.</summary>
    /// <param name="value">CNPJ do titular.</param>
    /// <returns>Nova instância de <see cref="CpfOrCnpj"/> com o CNPJ definido.</returns>
    public static CpfOrCnpj FromCnpj(Cnpj value) => new(value);

    /// <inheritdoc cref="FromCpf"/>
    public static explicit operator CpfOrCnpj(Cpf value) => FromCpf(value);

    /// <inheritdoc cref="FromCnpj"/>
    public static explicit operator CpfOrCnpj(Cnpj value) => FromCnpj(value);

    [XmlIgnore]
    string? ICpfOrCnpj.Cpf => Cpf?.ToString();

    [XmlIgnore]
    string? ICpfOrCnpj.Cnpj => Cnpj?.ToString();
}