using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Enums;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Tipo de documento do repositório nacional.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpDFeNacional</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class DocumentoFiscalNacional
{
    private TipoDocumentoFiscal? _tipoDocumentoFiscal;

    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DocumentoFiscalNacional() { }

    /// <summary>Inicializa a chave com os campos identificadores da NFS-e.</summary>
    /// <param name="tipoDocumentoFiscal">Tipo de documento fiscal.</param>
    /// <param name="chaveDocumentoFiscal">Chave do documento fiscal.</param>
    /// <param name="tipoChaveDfe">Tipo de chave DF-e. Opcional.</param>
    /// <exception cref="ArgumentNullException">Lançada quando <paramref name="chaveDocumentoFiscal"/> for nulo.</exception>
    /// <exception cref="ArgumentException">Lançada quando <paramref name="tipoChaveDfe"/> for nulo e <paramref name="tipoDocumentoFiscal"/> for 'Outro'.</exception>
    public DocumentoFiscalNacional(TipoDocumentoFiscal tipoDocumentoFiscal, ChaveDocumentoFiscal chaveDocumentoFiscal, TipoChaveDfe? tipoChaveDfe = null)
    {
        ArgumentNullException.ThrowIfNull(chaveDocumentoFiscal, nameof(chaveDocumentoFiscal));

        if (tipoDocumentoFiscal == Enums.TipoDocumentoFiscal.Outro && tipoChaveDfe == null)
        {
            throw new ArgumentException("O campo 'tipoChaveDfe' é obrigatório quando 'tipoDocumentoFiscal' for 'Outro'.", nameof(tipoChaveDfe));
        }

        TipoDocumentoFiscal = tipoDocumentoFiscal;
        TipoChaveDfe = tipoChaveDfe;
        ChaveDocumentoFiscal = chaveDocumentoFiscal;
    }

    /// <summary>Código de verificação da NFS-e. Opcional.</summary>
    [XmlElement("tipoChaveDFe", Form = XmlSchemaForm.Unqualified)]
    public TipoDocumentoFiscal? TipoDocumentoFiscal
    {
        get => _tipoDocumentoFiscal;
        set
        {
            _tipoDocumentoFiscal = value;
            TipoDocumentoFiscalSpecified = value.HasValue;
        }
    }

    /// <summary>Indica se o campo <see cref="TipoDocumentoFiscal"/> deve ser serializado. O campo é opcional, mas se for preenchido, a propriedade correspondente será incluída no XML.</summary>
    [XmlIgnore]
    public bool TipoDocumentoFiscalSpecified { get; set; }

    /// <summary>Número sequencial da NFS-e emitida.</summary>
    [XmlElement("xTipoChaveDFe", Form = XmlSchemaForm.Unqualified)]
    public TipoChaveDfe? TipoChaveDfe { get; set; }

    /// <summary>Chave do Documento Fiscal eletrônico do repositório nacional referenciado para os casos de operações já tributadas.</summary>
    [XmlElement("chaveDFe", Form = XmlSchemaForm.Unqualified)]
    public ChaveDocumentoFiscal? ChaveDocumentoFiscal { get; set; }
}