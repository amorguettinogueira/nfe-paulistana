using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Enums;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Tipo de documento referenciado nos casos de reembolso, repasse e ressarcimento que serão considerados na base de cálculo do ISSQN, do IBS e da CBS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpDocumento</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class Documento
{
    private TipoValorIncluso? _tipoValorIncluso;

    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Documento() { }

    /// <summary>Inicializa a chave com os campos identificadores do documento.</summary>
    /// <param name="dataEmissao">Data da emissão do documento dedutível. Ano, mês e dia (AAAA-MM-DD).</param>
    /// <param name="dataCompetencia">Data de competência do documento dedutível. Ano, mês e dia (AAAA-MM-DD).</param>
    /// <param name="tipoValorIncluso">Tipo de valor incluso no documento.</param>
    /// <param name="valorDocumento">Valor do documento dedutível.</param>
    /// <param name="documentoFiscalNacional">Tipo de documento do repositório nacional.</param>
    /// <param name="outroDocumentoFiscal">Grupo de informações de documento fiscais, eletrônicos ou não, que não se encontram no repositório nacional.</param>
    /// <param name="documentoNaoFiscal">Grupo de informações de documento não fiscal.</param>
    /// <param name="fornecedor">Grupo de informações do fornecedor do documento referenciado.</param>
    /// <param name="descricaoOutrosReembolsos">Descrição de outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro.</param>
    /// <exception cref="ArgumentNullException">Lançada quando <paramref name="dataEmissao"/>, <paramref name="dataCompetencia"/> ou <paramref name="valorDocumento"/> forem nulos.</exception>
    /// <exception cref="ArgumentException">Lançada quando nenhum dos campos <paramref name="documentoFiscalNacional"/>, <paramref name="outroDocumentoFiscal"/> ou <paramref name="documentoNaoFiscal"/> for preenchido, ou quando <paramref name="tipoValorIncluso"/> for '99 - Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro' e <paramref name="descricaoOutrosReembolsos"/> não for preenchido.</exception>
    public Documento(
        DataXsd dataEmissao,
        DataXsd dataCompetencia,
        TipoValorIncluso tipoValorIncluso,
        Valor valorDocumento,
        DocumentoFiscalNacional? documentoFiscalNacional,
        OutroDocumentoFiscal? outroDocumentoFiscal,
        DocumentoNaoFiscal? documentoNaoFiscal,
        Fornecedor? fornecedor,
        DescricaoOutrosReembolsos? descricaoOutrosReembolsos)
    {
        ArgumentNullException.ThrowIfNull(dataEmissao, nameof(dataEmissao));
        ArgumentNullException.ThrowIfNull(dataCompetencia, nameof(dataCompetencia));
        ArgumentNullException.ThrowIfNull(valorDocumento, nameof(valorDocumento));

        if (documentoFiscalNacional is null && outroDocumentoFiscal is null && documentoNaoFiscal is null)
        {
            throw new ArgumentException("Ao menos um dos campos DocumentoFiscalNacional, OutroDocumentoFiscal ou DocumentoNaoFiscal deve ser preenchido.");
        }

        if (tipoValorIncluso == Enums.TipoValorIncluso.OutrosReembolsos && descricaoOutrosReembolsos is null)
        {
            throw new ArgumentException("O campo DescricaoOutrosReembolsos deve ser preenchido quando TipoValorIncluso for '99 - Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro'.");
        }

        DataEmissao = dataEmissao;
        DataCompetencia = dataCompetencia;
        TipoValorIncluso = tipoValorIncluso;
        ValorDocumento = valorDocumento;
        DocumentoFiscalNacional = documentoFiscalNacional;
        OutroDocumentoFiscal = outroDocumentoFiscal;
        DocumentoNaoFiscal = documentoNaoFiscal;
        Fornecedor = fornecedor;
        DescricaoOutrosReembolsos = descricaoOutrosReembolsos;
    }

    /// <summary>Tipo de documento do repositório nacional.</summary>
    [XmlElement("dFeNacional", Form = XmlSchemaForm.Unqualified)]
    public DocumentoFiscalNacional? DocumentoFiscalNacional { get; set; }

    /// <summary>Grupo de informações de documento fiscais, eletrônicos ou não, que não se encontram no repositório nacional.</summary>
    [XmlElement("docFiscalOutro", Form = XmlSchemaForm.Unqualified)]
    public OutroDocumentoFiscal? OutroDocumentoFiscal { get; set; }

    /// <summary>Grupo de informações de documento não fiscal.</summary>
    [XmlElement("docOutro", Form = XmlSchemaForm.Unqualified)]
    public DocumentoNaoFiscal? DocumentoNaoFiscal { get; set; }

    /// <summary>Grupo de informações do fornecedor do documento referenciado.</summary>
    [XmlElement("fornec", Form = XmlSchemaForm.Unqualified)]
    public Fornecedor? Fornecedor { get; set; }

    /// <summary>Data da emissão do documento dedutível. Ano, mês e dia (AAAA-MM-DD).</summary>
    [XmlElement("dtEmiDoc", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DataEmissao { get; set; }

    /// <summary>Data da competência do documento dedutível. Ano, mês e dia (AAAA-MM-DD).</summary>
    [XmlElement("dtCompDoc", Form = XmlSchemaForm.Unqualified)]
    public DataXsd? DataCompetencia { get; set; }

    /// <summary>Tipo de valor incluído neste documento, recebido por motivo de estarem relacionadas a operações de terceiros, objeto de reembolso, repasse ou ressarcimento pelo recebedor, já tributados e aqui referenciado.</summary>
    [XmlElement("tpReeRepRes", Form = XmlSchemaForm.Unqualified)]
    public TipoValorIncluso? TipoValorIncluso
    {
        get => _tipoValorIncluso;
        set
        {
            _tipoValorIncluso = value;
            TipoValorInclusoSpecified = value.HasValue;
        }
    }

    /// <summary>Indica se o campo <see cref="TipoValorIncluso"/> deve ser serializado. O campo é opcional, mas se for preenchido, a propriedade correspondente será incluída no XML.</summary>
    [XmlIgnore]
    public bool TipoValorInclusoSpecified { get; set; }

    /// <summary>Descrição do reembolso ou ressarcimento quando <see cref="TipoValorIncluso"/> é "99 - Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro"</summary>
    [XmlElement("xTpReeRepRes", Form = XmlSchemaForm.Unqualified)]
    public DescricaoOutrosReembolsos? DescricaoOutrosReembolsos { get; set; }

    /// <summary>Valor monetário (total ou parcial, conforme documento informado) utilizado para não inclusão na base de cálculo do ISS e do IBS e da CBS da NFS-e que está sendo emitida (R$).</summary>
    [XmlElement("vlrReeRepRes", Form = XmlSchemaForm.Unqualified)]
    public Valor? ValorDocumento { get; set; }
}