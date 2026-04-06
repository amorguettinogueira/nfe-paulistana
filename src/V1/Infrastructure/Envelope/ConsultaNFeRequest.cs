using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de consulta de NFS-e (<c>ConsultaNFeRequest</c>) para o webservice
/// da NF-e Paulistana, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 1, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaNFeRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaNFeRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaNFeRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaNFeRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ConsultaNFeRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoConsultaNFe"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public ConsultaNFeRequest(MensagemXml<PedidoConsultaNFe> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 1.</summary>
    public int VersaoSchema { get; set; } = 1;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoConsultaNFe"/> para consulta.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoConsultaNFe>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="ConsultaNFeRequest"/> a partir de um <see cref="PedidoConsultaNFe"/>.
    /// </summary>
    /// <param name="value">Pedido de consulta de NFS-e.</param>
    public static ConsultaNFeRequest FromPedidoConsultaNFe(PedidoConsultaNFe value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoConsultaNFe>(value));
    }

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoConsultaNFe"/> para <see cref="ConsultaNFeRequest"/>.
    /// </summary>
    public static explicit operator ConsultaNFeRequest(PedidoConsultaNFe value) =>
        FromPedidoConsultaNFe(value);
}