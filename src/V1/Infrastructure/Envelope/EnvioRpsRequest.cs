using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de envio de RPS unitário (<c>EnvioRPSRequest</c>) para o webservice
/// da NF-e Paulistana, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 1, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "EnvioRPSRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class EnvioRpsRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="EnvioRpsRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EnvioRpsRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="EnvioRpsRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoEnvio"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public EnvioRpsRequest(MensagemXml<PedidoEnvio> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 1.</summary>
    public int VersaoSchema { get; set; } = 1;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoEnvio"/> para envio.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoEnvio>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="EnvioRpsRequest"/> a partir de um <see cref="PedidoEnvio"/>.
    /// </summary>
    /// <param name="value">Pedido de envio de RPS unitário.</param>
    public static EnvioRpsRequest FromPedidoEnvio(PedidoEnvio value) =>
        new(new MensagemXml<PedidoEnvio>(value));

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoEnvio"/> para <see cref="EnvioRpsRequest"/>.
    /// </summary>
    public static explicit operator EnvioRpsRequest(PedidoEnvio value) =>
        FromPedidoEnvio(value);
}