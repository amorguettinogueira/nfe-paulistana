using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Enums;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Tipo union que representa um documento fiscal brasileiro — CPF ou CNPJ ou NIF —
/// como exigido pelo XSD v02 da NF-e Paulistana (elemento <c>tpCPFCNPJNIF</c>).
/// Apenas um dos campos deve estar preenchido por instância.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCPFCNPJNIF</c>.
/// Utiliza o <see cref="Cnpj"/> alfanumérico do schema v02.
/// </remarks>
[Serializable]
public class CpfOrCnpjOrNif
{
    private MotivoNifNaoInformado? _motivoNifNaoInformado;

    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CpfOrCnpjOrNif()
    { }

    /// <summary>Inicializa com um CPF.</summary>
    /// <param name="cpf">CPF a ser armazenado.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="cpf"/> for nulo.</exception>
    public CpfOrCnpjOrNif(Cpf cpf)
    {
        ArgumentNullException.ThrowIfNull(cpf);
        Cpf = cpf;
    }

    /// <summary>Inicializa com um CNPJ.</summary>
    /// <param name="cnpj">CNPJ alfanumérico a ser armazenado.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="cnpj"/> for nulo.</exception>
    public CpfOrCnpjOrNif(Cnpj cnpj)
    {
        ArgumentNullException.ThrowIfNull(cnpj);
        Cnpj = cnpj;
    }

    /// <summary>
    /// Inicializa com um NIF (Número de Identificação Fiscal) — fornecido por um órgão de administração tributária no exterior.
    /// </summary>
    /// <param name="nif">NIF a ser armazenado.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nif"/> for nulo.</exception>
    public CpfOrCnpjOrNif(Nif nif)
    {
        ArgumentNullException.ThrowIfNull(nif);
        Nif = nif;
    }

    /// <summary>
    /// Inicializa com um motivo pelo qual o NIF não foi informado, caso aplicável.
    /// </summary>
    /// <param name="motivoNifNaoInformado">Motivo pelo qual o NIF não foi informado.</param>
    public CpfOrCnpjOrNif(MotivoNifNaoInformado motivoNifNaoInformado) =>
        MotivoNifNaoInformado = motivoNifNaoInformado;

    /// <summary>CPF do titular. Mutuamente exclusivo com <see cref="Cnpj"/>, <see cref="Nif"/> e <see cref="MotivoNifNaoInformado"/>.</summary>
    [XmlElement("CPF", Form = XmlSchemaForm.Unqualified)]
    public Cpf? Cpf { get; set; }

    /// <summary>CNPJ alfanumérico do titular. Mutuamente exclusivo com <see cref="Cpf"/>, <see cref="Nif"/> e <see cref="MotivoNifNaoInformado"/>.</summary>
    [XmlElement("CNPJ", Form = XmlSchemaForm.Unqualified)]
    public Cnpj? Cnpj { get; set; }

    /// <summary>NIF (Número de Identificação Fiscal) do titular. Mutuamente exclusivo com <see cref="Cpf"/>, <see cref="Cnpj"/> e <see cref="MotivoNifNaoInformado"/>.</summary>
    [XmlElement("NIF", Form = XmlSchemaForm.Unqualified)]
    public Nif? Nif { get; set; }

    /// <summary>Motivo pelo qual o NIF não foi informado, caso aplicável. Mutuamente exclusivo com <see cref="Cpf"/>, <see cref="Cnpj"/> e <see cref="Nif"/>.</summary>
    [XmlElement("NaoNIF", Form = XmlSchemaForm.Unqualified)]
    public MotivoNifNaoInformado? MotivoNifNaoInformado
    {
        get => _motivoNifNaoInformado;
        set
        {
            _motivoNifNaoInformado = value;
            MotivoNifNaoInformadoSpecified = value.HasValue;
        }
    }

    /// <summary>Indica se a propriedade <see cref="MotivoNifNaoInformado"/> foi definida, necessária para controle de serialização XML.</summary>
    [XmlIgnore]
    public bool MotivoNifNaoInformadoSpecified { get; set; }

    /// <summary>
    /// Retorna a representação textual do documento: CPF quando definido, CNPJ caso contrário.
    /// </summary>
    public override string? ToString() =>
        Cpf?.ToString() ?? Cnpj?.ToString() ?? Nif?.ToString() ?? MotivoNifNaoInformado?.ToString();

    /// <summary>Cria uma instância a partir de um CPF.</summary>
    /// <param name="value">CPF do titular.</param>
    /// <returns>Nova instância de <see cref="CpfOrCnpjOrNif"/> com o CPF definido.</returns>
    public static CpfOrCnpjOrNif FromCpf(Cpf value) => new(value);

    /// <summary>Cria uma instância a partir de um CNPJ alfanumérico.</summary>
    /// <param name="value">CNPJ alfanumérico do titular.</param>
    /// <returns>Nova instância de <see cref="CpfOrCnpjOrNif"/> com o CNPJ definido.</returns>
    public static CpfOrCnpjOrNif FromCnpj(Cnpj value) => new(value);

    /// <summary>Cria uma instância a partir de um NIF (Número de Identificação Fiscal) — fornecido por um órgão de administração tributária no exterior.</summary>
    /// <param name="value">NIF do titular.</param>
    /// <returns>Nova instância de <see cref="CpfOrCnpjOrNif"/> com o NIF definido.</returns>
    public static CpfOrCnpjOrNif FromNif(Nif value) => new(value);

    /// <summary>Cria uma instância a partir de um motivo pelo qual o NIF não foi informado.</summary>
    /// <param name="value">Motivo pelo qual o NIF não foi informado.</param>
    /// <returns>Nova instância de <see cref="CpfOrCnpjOrNif"/> com o motivo definido.</returns>
    public static CpfOrCnpjOrNif FromMotivoNifNaoInformado(MotivoNifNaoInformado value) => new(value);

    /// <inheritdoc cref="FromCpf"/>
    public static explicit operator CpfOrCnpjOrNif(Cpf value) => FromCpf(value);

    /// <inheritdoc cref="FromCnpj"/>
    public static explicit operator CpfOrCnpjOrNif(Cnpj value) => FromCnpj(value);

    /// <inheritdoc cref="FromNif"/>
    public static explicit operator CpfOrCnpjOrNif(Nif value) => FromNif(value);

    /// <inheritdoc cref="FromMotivoNifNaoInformado"/>
    public static explicit operator CpfOrCnpjOrNif(MotivoNifNaoInformado value) => FromMotivoNifNaoInformado(value);
}