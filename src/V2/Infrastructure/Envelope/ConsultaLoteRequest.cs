using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de consulta de lote (<c>ConsultaLoteRequest</c>) para o webservice
/// da NF-e Paulistana v02, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 2, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaLoteRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaLoteRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaLoteRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaLoteRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ConsultaLoteRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoConsultaLote"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public ConsultaLoteRequest(MensagemXml<PedidoConsultaLote> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 2.</summary>
    public int VersaoSchema { get; set; } = 2;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoConsultaLote"/> para consulta de lote.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoConsultaLote>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="ConsultaLoteRequest"/> a partir de um <see cref="PedidoConsultaLote"/>.
    /// </summary>
    /// <param name="value">Pedido de consulta de lote.</param>
    public static ConsultaLoteRequest FromPedidoConsultaLote(PedidoConsultaLote value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoConsultaLote>(value));
    }

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoConsultaLote"/> para <see cref="ConsultaLoteRequest"/>.
    /// </summary>
    public static explicit operator ConsultaLoteRequest(PedidoConsultaLote value) =>
        FromPedidoConsultaLote(value);
}