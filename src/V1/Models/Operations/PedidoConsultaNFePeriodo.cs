using Nfe.Paulistana.Models;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Operations;

/// <summary>
/// Define o corpo da requisição de consulta de NFS-e por período (<c>PedidoConsultaNFePeriodo</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoConsultaNFePeriodo_v01.xsd</c> — Elemento <c>PedidoConsultaNFePeriodo</c>.
/// </para>
/// <para>
/// Compartilhado pelas operações <c>ConsultaNFeRecebidas</c> e <c>ConsultaNFeEmitidas</c>.
/// Instâncias devem ser criadas via
/// <see cref="Nfe.Paulistana.Builders.PedidoConsultaNFePeriodoFactory"/>, que garante
/// que o pedido esteja corretamente assinado antes do envio.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoConsultaNFePeriodo", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoConsultaNFePeriodo : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV1(Constants.Uris.Nfe, "PedidoConsultaNFePeriodo_v01.xsd");

    /// <summary>Cabeçalho da requisição contendo filtros de período, CPF/CNPJ e paginação.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CabecalhoConsultaPeriodo? Cabecalho { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}
