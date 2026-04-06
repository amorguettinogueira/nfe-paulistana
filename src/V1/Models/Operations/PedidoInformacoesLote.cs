using Nfe.Paulistana.Models;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Operations;

/// <summary>
/// Define o corpo da requisição de informações de lote (<c>PedidoInformacoesLote</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoInformacoesLote_v01.xsd</c> — Elemento <c>PedidoInformacoesLote</c>.
/// </para>
/// <para>
/// Permite obter informações sobre lotes de RPS que geraram NFS-e.
/// Instâncias devem ser criadas via
/// <see cref="Builders.PedidoInformacoesLoteFactory"/>, que garante
/// que o pedido esteja corretamente assinado antes do envio.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoInformacoesLote", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoInformacoesLote : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV1(Constants.Uris.Nfe, "PedidoInformacoesLote_v01.xsd");

    /// <summary>
    /// Cabeçalho da requisição contendo o CPF ou CNPJ do remetente,
    /// número do lote (opcional) e inscrição municipal do prestador.
    /// </summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CabecalhoInformacoesLote? Cabecalho { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}