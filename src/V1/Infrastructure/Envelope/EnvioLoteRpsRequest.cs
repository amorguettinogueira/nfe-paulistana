using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de envio de lote de RPS (<c>EnvioLoteRPSRequest</c>) para o webservice
/// da NF-e Paulistana, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 1, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioLoteRPSRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioLoteRpsRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="EnvioLoteRpsRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioLoteRpsRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="EnvioLoteRpsRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoEnvioLote"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public EnvioLoteRpsRequest(MensagemXml<PedidoEnvioLote> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 1.</summary>
    public int VersaoSchema { get; set; } = 1;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoEnvioLote"/> para envio em lote.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoEnvioLote>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="EnvioLoteRpsRequest"/> a partir de um <see cref="PedidoEnvioLote"/>.
    /// </summary>
    /// <param name="value">Pedido de envio em lote de RPS.</param>
    public static EnvioLoteRpsRequest FromPedidoEnvioLote(PedidoEnvioLote value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoEnvioLote>(value));
    }

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoEnvioLote"/> para <see cref="EnvioLoteRpsRequest"/>.
    /// </summary>
    public static explicit operator EnvioLoteRpsRequest(PedidoEnvioLote value) =>
        FromPedidoEnvioLote(value);
}