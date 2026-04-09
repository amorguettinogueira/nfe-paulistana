using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Enums;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Modelo de dados de Tipo das informações do IBS/CBS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpIBSCBS</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class InformacoesIbsCbs
{
    private TipoOperacao? _tipoOperacao;
    private TipoEnteGovernamental? _enteGovernamental;

    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InformacoesIbsCbs()
    { }

    /// <summary>Inicializa a instância com as informações de IBS/CBS especificadas.</summary>
    /// <param name="finalidadeEmissaoNfe">Finalidade da emissão de NFS-e.</param>
    /// <param name="usoOuConsumoPessoal">Indica operação de uso ou consumo pessoal.</param>
    /// <param name="destinatarioServicos">Indica o Destinatário dos serviços.</param>
    /// <param name="valores">Valores relacionados às informações de IBS/CBS.</param>
    /// <param name="codigoOperacaoFornecimento">Código indicador da operação de fornecimento.</param>
    /// <param name="tipoOperacao">Tipo de Operação com Entes Governamentais ou outros serviços sobre bens imóveis.</param>
    /// <param name="grupoNFSe">Grupo de NFS-e referenciadas.</param>
    /// <param name="enteGovernamental">Tipo do ente da compra governamental.</param>
    /// <param name="destinatario">Informações do destinatário.</param>
    /// <param name="imovelObra">Informações do imóvel ou obra.</param>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="codigoOperacaoFornecimento"/> ou <paramref name="valores"/> forem nulos.</exception>
    public InformacoesIbsCbs(
        FinalidadeEmissaoNfe finalidadeEmissaoNfe,
        NaoSim usoOuConsumoPessoal,
        DestinatarioServicos destinatarioServicos,
        Valores valores,
        CodigoOperacao codigoOperacaoFornecimento,
        TipoOperacao? tipoOperacao = null,
        GrupoNfeReferenciada? grupoNFSe = null,
        TipoEnteGovernamental? enteGovernamental = null,
        InformacoesPessoa? destinatario = null,
        ImovelObra? imovelObra = null)
    {
        ArgumentNullException.ThrowIfNull(usoOuConsumoPessoal, nameof(usoOuConsumoPessoal));
        ArgumentNullException.ThrowIfNull(codigoOperacaoFornecimento, nameof(codigoOperacaoFornecimento));
        ArgumentNullException.ThrowIfNull(valores, nameof(valores));

        FinalidadeEmissao = finalidadeEmissaoNfe;
        UsoOuConsumoPessoal = usoOuConsumoPessoal;
        CodigoOperacaoFornecimento = codigoOperacaoFornecimento;
        TipoOperacao = tipoOperacao;
        NfesReferenciadas = grupoNFSe;
        EnteGovernamental = enteGovernamental;
        DestinatarioServicos = destinatarioServicos;
        Destinatario = destinatario;
        Valores = valores;
        ImovelObra = imovelObra;
    }

    /// <summary>Indicador da finalidade da emissão de NFS-e.</summary>
    [XmlElement("finNFSe", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public FinalidadeEmissaoNfe FinalidadeEmissao { get; set; } = FinalidadeEmissaoNfe.NfseRegular;

    /// <summary>Indica operação de uso ou consumo pessoal.</summary>
    [XmlElement("indFinal", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public NaoSim? UsoOuConsumoPessoal { get; set; }

    /// <summary>Código indicador da operação de fornecimento, conforme tabela "código indicador de operação" publicada no ANEXO AnexoVII-IndOp_IBSCBS_V1.00.00.xlsx.</summary>
    [XmlElement("cIndOp", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public CodigoOperacao? CodigoOperacaoFornecimento { get; set; }

    /// <summary>Tipo de Operação com Entes Governamentais ou outros serviços sobre bens imóveis.</summary>
    [XmlElement("tpOper", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public TipoOperacao? TipoOperacao
    {
        get => _tipoOperacao;
        set
        {
            _tipoOperacao = value;
            TipoOperacaoSpecified = value.HasValue;
        }
    }

    /// <summary>Indica se o campo <see cref="TipoOperacao"/> deve ser serializado. O campo é opcional, mas se for preenchido, a propriedade correspondente será incluída no XML.</summary>
    [XmlIgnore]
    public bool TipoOperacaoSpecified { get; set; }

    /// <summary>Grupo de NFS-e referenciadas.</summary>
    [XmlElement("gRefNFSe", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public GrupoNfeReferenciada? NfesReferenciadas { get; set; }

    /// <summary>Tipo do ente da compra governamental.</summary>
    [XmlElement("tpEnteGov", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public TipoEnteGovernamental? EnteGovernamental
    {
        get => _enteGovernamental;
        set
        {
            _enteGovernamental = value;
            EnteGovernamentalSpecified = value.HasValue;
        }
    }

    /// <summary>Indica se o campo <see cref="EnteGovernamental"/> deve ser serializado. O campo é opcional, mas se for preenchido, a propriedade correspondente será incluída no XML.</summary>
    [XmlIgnore]
    public bool EnteGovernamentalSpecified { get; set; }

    /// <summary>Indica o Destinatário dos serviços.</summary>
    [XmlElement("indDest", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public DestinatarioServicos DestinatarioServicos { get; set; } = DestinatarioServicos.ProprioTomador;

    /// <summary>Destinatário.</summary>
    [XmlElement("dest", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public InformacoesPessoa? Destinatario { get; set; }

    /// <summary>Informações relacionadas aos valores do serviço prestado para IBS e à CBS.</summary>
    [XmlElement("valores", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Valores? Valores { get; set; }

    /// <summary>Informações sobre o Tipo de Imóvel/Obra.</summary>
    [XmlElement("imovelobra", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public ImovelObra? ImovelObra { get; set; }
}