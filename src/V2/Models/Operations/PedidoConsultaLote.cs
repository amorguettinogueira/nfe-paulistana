using Nfe.Paulistana.Models;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Operations;

/// <summary>
/// Define o corpo da requisição de consulta de lote v02 (<c>PedidoConsultaLote</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoConsultaLote_v02.xsd</c> — Elemento <c>PedidoConsultaLote</c>.
/// </para>
/// <para>
/// Permite consultar as NFS-e geradas a partir de um lote de RPS.
/// Instâncias devem ser criadas via
/// <see cref="Nfe.Paulistana.V2.Builders.PedidoConsultaLoteFactory"/>, que garante
/// que o pedido esteja corretamente assinado antes do envio.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoConsultaLote", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoConsultaLote : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV2(Constants.Uris.Nfe, "PedidoConsultaLote_v02.xsd");

    /// <summary>Cabeçalho da requisição contendo o CPF ou CNPJ do remetente e o número do lote.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CabecalhoConsultaLote? Cabecalho { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}
