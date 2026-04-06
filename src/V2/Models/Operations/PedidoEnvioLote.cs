using Nfe.Paulistana.Models;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.Xml;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Models.Operations;

[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "PedidoEnvioLoteRPS", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[Serializable]
public sealed class PedidoEnvioLote : ISignedXmlFile, IXmlValidatableSchema
{
    private static readonly XmlSchemaSet _validationSchema =
        SchemaProvider.GetSchemaSetV2(Constants.Uris.Nfe, "PedidoEnvioLoteRPS_v02.xsd");

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public CabecalhoLote? Cabecalho { get; set; }

    [XmlElement(ElementName = "RPS", Form = XmlSchemaForm.Unqualified)]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Necessário para serialização XML sequencial dos elementos RPS; atribuÍdo pela fábrica.")]
    public Rps[]? Rps { get; set; }

    [XmlIgnore]
    public string? SignedXmlContent { get; set; }

    public XmlSchemaSet ValidationSchema => _validationSchema;
}
