using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Enums;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Tipo de informações de pessoa.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpInformacoesPessoa</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class InformacoesPessoa : Fornecedor
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InformacoesPessoa() { }

    /// <summary>
    /// Inicializa a chave com CPF, nome e demais informações opcionais.
    /// </summary>
    /// <param name="cpf">CPF da pessoa.</param>
    /// <param name="nome">Nome da pessoa.</param>
    /// <param name="endereco">Endereço da pessoa.</param>
    /// <param name="email">Email da pessoa.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public InformacoesPessoa(
        Cpf cpf,
        RazaoSocial nome,
        EnderecoSimplesIBSCBS? endereco = null,
        Email? email = null) : base(cpf, nome)
    {
        Endereco = endereco;
        Email = email;
    }

    /// <summary>
    /// Inicializa a chave com CNPJ, nome e demais informações opcionais.
    /// </summary>
    /// <param name="cnpj">CNPJ da pessoa.</param>
    /// <param name="nome">Nome da pessoa.</param>
    /// <param name="endereco">Endereço da pessoa.</param>
    /// <param name="email">Email da pessoa.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public InformacoesPessoa(
        Cnpj cnpj,
        RazaoSocial nome,
        EnderecoSimplesIBSCBS? endereco = null,
        Email? email = null) : base(cnpj, nome)
    {
        Endereco = endereco;
        Email = email;
    }

    /// <summary>
    /// Inicializa a chave com NIF, nome e demais informações opcionais.
    /// </summary>
    /// <param name="nif">NIF da pessoa.</param>
    /// <param name="nome">Nome da pessoa.</param>
    /// <param name="endereco">Endereço da pessoa.</param>
    /// <param name="email">Email da pessoa.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public InformacoesPessoa(
        Nif nif,
        RazaoSocial nome,
        EnderecoSimplesIBSCBS? endereco = null,
        Email? email = null) : base(nif, nome)
    {
        Endereco = endereco;
        Email = email;
    }

    /// <summary>
    /// Inicializa a chave com motivo de NIF não informado, nome e demais informações opcionais.
    /// </summary>
    /// <param name="motivoNifNaoInformado">Motivo pelo qual o NIF não foi informado.</param>
    /// <param name="nome">Nome da pessoa.</param>
    /// <param name="endereco">Endereço da pessoa.</param>
    /// <param name="email">Email da pessoa.</param>
    /// <exception cref="ArgumentNullException">Se <paramref name="nome"/> for nulo.</exception>
    public InformacoesPessoa(
        MotivoNifNaoInformado motivoNifNaoInformado,
        RazaoSocial nome,
        EnderecoSimplesIBSCBS? endereco = null,
        Email? email = null) : base(motivoNifNaoInformado, nome)
    {
        Endereco = endereco;
        Email = email;
    }

    /// <summary>Endereço.</summary>
    [XmlElement("end", Form = XmlSchemaForm.Unqualified)]
    public EnderecoSimplesIBSCBS? Endereco { get; set; }

    /// <summary>Endereço eletrônico.</summary>
    [XmlElement("email", Form = XmlSchemaForm.Unqualified)]
    public Email? Email { get; set; }
}