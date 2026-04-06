using Nfe.Paulistana.Models;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Operations;

/// <summary>
/// Define o corpo da requisição de envio individual de RPS (<c>PedidoEnvioRPS</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoEnvioRPS_v01.xsd</c> — Elemento <c>PedidoEnvioRPS</c>, linha 8.
/// </para>
/// <para>
/// Representa uma solicitação de envio de um único RPS à Prefeitura de São Paulo.
/// Instâncias devem ser criadas exclusivamente via
/// <see cref="Builders.PedidoEnvioFactory"/>, que garante
/// que o RPS e o pedido estejam corretamente assinados antes do envio.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoEnvioRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoEnvio : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV1(Constants.Uris.Nfe, "PedidoEnvioRPS_v01.xsd");

    /// <summary>Cabeçalho da requisição contendo o CPF ou CNPJ do prestador de serviços.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Cabecalho? Cabecalho { get; set; }

    /// <summary>RPS (Recibo Provisório de Serviços) a ser enviado, incluindo sua assinatura digital.</summary>
    [XmlElement(ElementName = "RPS", Form = XmlSchemaForm.Unqualified)]
    public Rps? Rps { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}