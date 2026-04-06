using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Enums;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Grupo de informações do fornecedor do documento referenciado.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpFornecedor</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public class Fornecedor : CpfOrCnpjOrNif
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Fornecedor() { }

    /// <summary>
    /// Inicializa a chave com CPF e nome.
    /// </summary>
    /// <param name="cpf">CPF da pessoa.</param>
    /// <param name="nome">Nome do fornecedor.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public Fornecedor(Cpf cpf, RazaoSocial nome) : base(cpf)
    {
        ArgumentNullException.ThrowIfNull(nome, nameof(nome));

        Nome = nome;
    }

    /// <summary>
    /// Inicializa a chave com CNPJ e nome.
    /// </summary>
    /// <param name="cnpj">CNPJ da pessoa.</param>
    /// <param name="nome">Nome do fornecedor.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public Fornecedor(Cnpj cnpj, RazaoSocial nome) : base(cnpj)
    {
        ArgumentNullException.ThrowIfNull(nome, nameof(nome));

        Nome = nome;
    }

    /// <summary>
    /// Inicializa a chave com NIF e nome.
    /// </summary>
    /// <param name="nif">NIF da pessoa.</param>
    /// <param name="nome">Nome do fornecedor.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public Fornecedor(Nif nif, RazaoSocial nome) : base(nif)
    {
        ArgumentNullException.ThrowIfNull(nome, nameof(nome));

        Nome = nome;
    }

    /// <summary>
    /// Inicializa a chave com motivo de NIF não informado e nome.
    /// </summary>
    /// <param name="motivoNifNaoInformado">Motivo pelo qual o NIF não foi informado.</param>
    /// <param name="nome">Nome do fornecedor.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public Fornecedor(MotivoNifNaoInformado motivoNifNaoInformado, RazaoSocial nome) : base(motivoNifNaoInformado)
    {
        ArgumentNullException.ThrowIfNull(nome, nameof(nome));

        Nome = nome;
    }

    /// <summary>Nome do fornecedor.</summary>
    [XmlElement("xNome", Form = XmlSchemaForm.Unqualified)]
    public RazaoSocial? Nome { get; set; }
}