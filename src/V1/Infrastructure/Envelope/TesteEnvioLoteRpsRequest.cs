using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de envio de lote de RPS em modo de teste (<c>TesteEnvioLoteRPSRequest</c>)
/// para o webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// Estruturalmente idêntico a <see cref="EnvioLoteRpsRequest"/>, diferindo apenas no nome do
/// elemento raiz. Utilizar este tipo permite testar a integração com o webservice sem impacto
/// nos registros fiscais. A versão do esquema é sempre 1, conforme o XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "TesteEnvioLoteRPSRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class TesteEnvioLoteRpsRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="TesteEnvioLoteRpsRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TesteEnvioLoteRpsRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="TesteEnvioLoteRpsRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoEnvioLote"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public TesteEnvioLoteRpsRequest(MensagemXml<PedidoEnvioLote> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 1.</summary>
    public int VersaoSchema { get; set; } = 1;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoEnvioLote"/> para envio em lote de teste.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoEnvioLote>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="TesteEnvioLoteRpsRequest"/> a partir de um <see cref="PedidoEnvioLote"/>.
    /// </summary>
    /// <param name="value">Pedido de envio em lote de RPS.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="value"/> é nulo.</exception>
    public static TesteEnvioLoteRpsRequest FromPedidoEnvioLote(PedidoEnvioLote value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoEnvioLote>(value));
    }

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoEnvioLote"/> para <see cref="TesteEnvioLoteRpsRequest"/>.
    /// </summary>
    public static explicit operator TesteEnvioLoteRpsRequest(PedidoEnvioLote value) =>
        FromPedidoEnvioLote(value);
}