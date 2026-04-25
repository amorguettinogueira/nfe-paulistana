using Nfe.Paulistana.Models;
using Nfe.Paulistana.V1.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Models.Operations;

/// <summary>
/// Define o corpo da requisição de envio em lote de RPS (<c>PedidoEnvioLoteRPS</c>).
/// Implementa <see cref="ISignedXmlFile"/> para armazenamento do XML assinado e
/// <see cref="IXmlValidatableSchema"/> para validação contra o XSD correspondente.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>PedidoEnvioLoteRPS_v01.xsd</c> — Elemento <c>PedidoEnvioLoteRPS</c>, linha 8.
/// </para>
/// <para>
/// Representa uma solicitação de envio em lote contendo múltiplos RPS à Prefeitura de São Paulo.
/// Instâncias devem ser criadas exclusivamente via
/// <see cref="Builders.PedidoEnvioLoteFactory"/>, que garante que todos
/// os RPS estejam individualmente assinados, as totalizações calculadas e o lote assinado.
/// </para>
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoEnvioLoteRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoEnvioLote : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV1(Constants.Uris.Nfe, "PedidoEnvioLoteRPS_v01.xsd");

    /// <summary>Cabeçalho do lote contendo identificação do prestador, período de competência e totalizações calculadas.</summary>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CabecalhoLote? Cabecalho { get; set; }

    /// <summary>Array de RPS (Recibos Provisórios de Serviços) contidos no lote, cada um com sua assinatura digital.</summary>
    [XmlElement(ElementName = "RPS", Form = XmlSchemaForm.Unqualified)]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML sequencial dos elementos RPS; atribuído pela PedidoEnvioLoteFactory.")]
    public Rps[]? Rps { get; set; }

    /// <inheritdoc/>
    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    /// <inheritdoc/>
    public XmlSchemaSet ValidationSchema => _validationSchema;
}