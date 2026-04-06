using Nfe.Paulistana.Models;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Operations;

/// <summary>
/// Define o corpo da requisição de envio individual de RPS v02 (<c>PedidoEnvioRPS</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// Fonte: <c>PedidoEnvioRPS_v02.xsd</c> — Elemento <c>PedidoEnvioRPS</c>.
/// Representa uma solicitação de envio de um único RPS à Prefeitura de São Paulo.
/// Instâncias devem ser criadas via <see cref="Builders.PedidoEnvioFactory"/>.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoEnvioRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoEnvio : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV2(Constants.Uris.Nfe, "PedidoEnvioRPS_v02.xsd");

    /// <summary>Cabeçalho da requisição contendo o CPF ou CNPJ do prestador de serviços.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public Domain.Cabecalho? Cabecalho { get; set; }

    /// <summary>RPS (Recibo Provisório de Serviços) a ser enviado, incluindo sua assinatura digital.</summary>
    [XmlElement(ElementName = "RPS", Form = XmlSchemaForm.Unqualified)]
    public Rps? Rps { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}