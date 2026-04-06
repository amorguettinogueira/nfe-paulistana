using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Grupo de informações de documento fiscais, eletrônicos ou não, que não se encontram no repositório nacional.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpDocFiscalOutro</c>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class OutroDocumentoFiscal
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public OutroDocumentoFiscal() { }

    /// <summary>Inicializa a chave com os campos identificadores do documento fiscal que não se encontra no repositório nacional.</summary>
    /// <param name="codigoMunicipio">Código do município emissor do documento fiscal.</param>
    /// <param name="numeroDocumento">Número do documento fiscal.</param>
    /// <param name="descricaoDocumento">Descrição do documento fiscal.</param>
    /// <exception cref="ArgumentNullException">Lançada quando algum dos parâmetros for nulo.</exception>
    public OutroDocumentoFiscal(CodigoIbge codigoMunicipio, NumeroDescricaoDocumento numeroDocumento, NumeroDescricaoDocumento descricaoDocumento)
    {
        ArgumentNullException.ThrowIfNull(codigoMunicipio, nameof(codigoMunicipio));
        ArgumentNullException.ThrowIfNull(numeroDocumento, nameof(numeroDocumento));
        ArgumentNullException.ThrowIfNull(descricaoDocumento, nameof(descricaoDocumento));

        CodigoMunicipio = codigoMunicipio;
        NumeroDocumento = numeroDocumento;
        DescricaoDocumento = descricaoDocumento;
    }

    /// <summary>Código do município emissor do documento fiscal que não se encontra no repositório nacional.</summary>
    [XmlElement("cMunDocFiscal", Form = XmlSchemaForm.Unqualified)]
    public CodigoIbge? CodigoMunicipio { get; set; }

    /// <summary>Número do documento fiscal que não se encontra no repositório nacional.</summary>
    [XmlElement("nDocFiscal", Form = XmlSchemaForm.Unqualified)]
    public NumeroDescricaoDocumento? NumeroDocumento { get; set; }

    /// <summary>Descrição do documento fiscal.</summary>
    [XmlElement("xDocFiscal", Form = XmlSchemaForm.Unqualified)]
    public NumeroDescricaoDocumento? DescricaoDocumento { get; set; }
}