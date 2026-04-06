using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Domain;

/// <summary>
/// Chave de identificação única de uma NFS-e, composta pela Inscrição Municipal
/// do prestador, o número da NFS-e e, opcionalmente, o código de verificação
/// e a chave da Nota Nacional.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpChaveNFe</c>.
/// Utilizado no contexto de requisição (ex.: <c>PedidoConsultaNFe</c>).
/// Diferente da v01, a v02 inclui o campo opcional <see cref="ChaveNotaNacional"/>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.NfeTipos)]
[Serializable]
public sealed class ChaveNfe
{
    /// <summary>Inicializa uma instância vazia. Necessário para serialização XML.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ChaveNfe() { }

    /// <summary>Inicializa a chave com os campos identificadores da NFS-e.</summary>
    /// <param name="inscricaoPrestador">Inscrição Municipal do prestador emissor da NFS-e.</param>
    /// <param name="numeroNFe">Número sequencial da NFS-e emitida.</param>
    /// <param name="codigoVerificacao">Código de verificação da NFS-e. Opcional.</param>
    /// <param name="chaveNotaNacional">Chave da Nota Nacional (50 caracteres alfanuméricos maiúsculos). Opcional.</param>
    public ChaveNfe(InscricaoMunicipal inscricaoPrestador, Numero numeroNFe, CodigoVerificacao? codigoVerificacao = null, ChaveNotaNacional? chaveNotaNacional = null)
    {
        InscricaoPrestador = inscricaoPrestador;
        NumeroNFe = numeroNFe;
        CodigoVerificacao = codigoVerificacao;
        ChaveNotaNacional = chaveNotaNacional;
    }

    /// <summary>Inscrição Municipal do prestador emissor da NFS-e.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public InscricaoMunicipal? InscricaoPrestador { get; set; }

    /// <summary>Número sequencial da NFS-e emitida.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Numero? NumeroNFe { get; set; }

    /// <summary>Código de verificação da NFS-e. Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CodigoVerificacao? CodigoVerificacao { get; set; }

    /// <summary>Chave da Nota Nacional (50 caracteres alfanuméricos maiúsculos). Opcional.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public ChaveNotaNacional? ChaveNotaNacional { get; set; }
}