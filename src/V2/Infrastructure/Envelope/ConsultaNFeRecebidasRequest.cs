using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de consulta de NFS-e recebidas (<c>ConsultaNFeRecebidasRequest</c>) para o
/// webservice da NF-e Paulistana v02, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 2, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaNFeRecebidasRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaNFeRecebidasRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaNFeRecebidasRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaNFeRecebidasRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ConsultaNFeRecebidasRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoConsultaNFePeriodo"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public ConsultaNFeRecebidasRequest(MensagemXml<PedidoConsultaNFePeriodo> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 2.</summary>
    public int VersaoSchema { get; set; } = 2;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoConsultaNFePeriodo"/> para consulta de NFS-e recebidas.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoConsultaNFePeriodo>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="ConsultaNFeRecebidasRequest"/> a partir de um <see cref="PedidoConsultaNFePeriodo"/>.
    /// </summary>
    /// <param name="value">Pedido de consulta de NFS-e por período.</param>
    public static ConsultaNFeRecebidasRequest FromPedidoConsultaNFePeriodo(PedidoConsultaNFePeriodo value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoConsultaNFePeriodo>(value));
    }

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoConsultaNFePeriodo"/> para <see cref="ConsultaNFeRecebidasRequest"/>.
    /// </summary>
    public static explicit operator ConsultaNFeRecebidasRequest(PedidoConsultaNFePeriodo value) =>
        FromPedidoConsultaNFePeriodo(value);
}