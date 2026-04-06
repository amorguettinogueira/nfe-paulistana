using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Tipo de imovel/obra.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpImovelObra</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class ImovelObra
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ImovelObra() { }

    /// <summary>Inicializa a instância com os campos identificadores do imóvel ou obra.</summary>
    /// <param name="cadastroImovel">Cadastro do imóvel.</param>
    /// <param name="identificacaoObra">Identificação da obra.</param>
    /// <param name="endereco">Endereço simplificado do imóvel ou obra.</param>
    /// <param name="inscricaoImobiliaria">Inscrição Imobiliária fiscal (código fornecido pela prefeitura para identificação da obra ou para fins de recolhimento do IPTU). Exemplos: SQL ou INCRA.</param>
    /// <exception cref="ArgumentException">Lançada quando nenhum dos campos <paramref name="cadastroImovel"/>, <paramref name="identificacaoObra"/> ou <paramref name="endereco"/> é fornecido.</exception>
    public ImovelObra(
        CadastroImovel? cadastroImovel = null,
        IdentificacaoObra? identificacaoObra = null,
        EnderecoSimplesIBSCBS? endereco = null,
        InscricaoImobiliaria? inscricaoImobiliaria = null)
    {
        if (cadastroImovel == null && identificacaoObra == null && endereco == null)
        {
            throw new ArgumentException("É necessário fornecer ao menos um dos campos: Cadastro de Imóvel, Identificação da Obra ou Endereço.");
        }

        CadastroImovel = cadastroImovel;
        IdentificacaoObra = identificacaoObra;
        Endereco = endereco;
        InscricaoImobiliaria = inscricaoImobiliaria;
    }

    /// <summary>Inscrição Imobiliária fiscal (código fornecido pela prefeitura para identificação da obra ou para fins de recolhimento do IPTU). Exemplos: SQL ou INCRA.</summary>
    [XmlElement("inscImobFisc", Form = XmlSchemaForm.Unqualified)]
    public InscricaoImobiliaria? InscricaoImobiliaria { get; set; }

    /// <summary>Cadastro de imóveis.</summary>
    [XmlElement("cCIB", Form = XmlSchemaForm.Unqualified)]
    public CadastroImovel? CadastroImovel { get; set; }

    /// <summary>Identificação da obra.</summary>
    [XmlElement("cObra", Form = XmlSchemaForm.Unqualified)]
    public IdentificacaoObra? IdentificacaoObra { get; set; }

    /// <summary>Endereço simplificado do imóvel ou obra.</summary>
    [XmlElement("end", Form = XmlSchemaForm.Unqualified)]
    public EnderecoSimplesIBSCBS? Endereco { get; set; }
}