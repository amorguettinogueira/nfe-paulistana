using Nfe.Paulistana.Models;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Operations;

/// <summary>
/// Define o corpo da requisição de consulta de CNPJ v02 (<c>PedidoConsultaCNPJ</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoConsultaCNPJ_v02.xsd</c> — Elemento <c>PedidoConsultaCNPJ</c>.
/// </para>
/// <para>
/// Permite consultar quais Inscrições Municipais (CCM) estão vinculadas a um
/// determinado CNPJ e se estes CCM emitem NFS-e.
/// Instâncias devem ser criadas via
/// <see cref="Nfe.Paulistana.V2.Builders.PedidoConsultaCNPJFactory"/>, que garante
/// que o pedido esteja corretamente assinado antes do envio.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoConsultaCNPJ", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoConsultaCNPJ : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV2(Constants.Uris.Nfe, "PedidoConsultaCNPJ_v02.xsd");

    /// <summary>Cabeçalho da requisição contendo o CPF ou CNPJ do remetente.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Cabecalho? Cabecalho { get; set; }

    /// <summary>
    /// CPF ou CNPJ do contribuinte que se deseja consultar.
    /// </summary>
    [XmlElement("CNPJContribuinte", Form = XmlSchemaForm.Unqualified)]
    public CpfOrCnpj? CnpjContribuinte { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}
