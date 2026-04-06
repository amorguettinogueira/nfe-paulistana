using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V1.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de consulta de CNPJ (<c>ConsultaCNPJRequest</c>) para o webservice
/// da NF-e Paulistana, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 1, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "ConsultaCNPJRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConsultaCNPJRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="ConsultaCNPJRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConsultaCNPJRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ConsultaCNPJRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoConsultaCNPJ"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public ConsultaCNPJRequest(MensagemXml<PedidoConsultaCNPJ> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 1.</summary>
    public int VersaoSchema { get; set; } = 1;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoConsultaCNPJ"/> para consulta de CNPJ.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoConsultaCNPJ>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="ConsultaCNPJRequest"/> a partir de um <see cref="PedidoConsultaCNPJ"/>.
    /// </summary>
    /// <param name="value">Pedido de consulta de CNPJ.</param>
    public static ConsultaCNPJRequest FromPedidoConsultaCNPJ(PedidoConsultaCNPJ value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoConsultaCNPJ>(value));
    }

    /// <summary>
    /// Operador de conversão explícita de <see cref="PedidoConsultaCNPJ"/> para <see cref="ConsultaCNPJRequest"/>.
    /// </summary>
    public static explicit operator ConsultaCNPJRequest(PedidoConsultaCNPJ value) =>
        FromPedidoConsultaCNPJ(value);
}