using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Models.Operations;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Nfe.Paulistana.V2.Infrastructure.Envelope;

/// <summary>
/// Representa a requisição de cancelamento de NFS-e (<c>CancelamentoNFeRequest</c>) para o
/// webservice da NF-e Paulistana v02, contendo a versão do esquema e a mensagem XML assinada.
/// </summary>
/// <remarks>
/// Este tipo é o elemento raiz quando transmitido dentro do elemento Body de um envelope SOAP.
/// A versão do esquema é sempre 2, conforme definido pelo XSD da Prefeitura de São Paulo.
/// </remarks>
[XmlType(AnonymousType = true, Namespace = Constants.Uris.Nfe)]
[XmlRoot(ElementName = "CancelamentoNFeRequest", Namespace = Constants.Uris.Nfe, IsNullable = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class CancelamentoNFeRequest
{
    /// <summary>Inicializa uma nova instância de <see cref="CancelamentoNFeRequest"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CancelamentoNFeRequest() { }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CancelamentoNFeRequest"/> com a mensagem XML especificada.
    /// </summary>
    /// <param name="mensagemXml">Mensagem XML contendo o <see cref="PedidoCancelamentoNFe"/> assinado.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="mensagemXml"/> é nulo.</exception>
    public CancelamentoNFeRequest(MensagemXml<PedidoCancelamentoNFe> mensagemXml)
    {
        ArgumentNullException.ThrowIfNull(mensagemXml);
        MensagemXml = mensagemXml;
    }

    /// <summary>Versão do esquema XSD utilizado. Valor fixo: 2.</summary>
    public int VersaoSchema { get; set; } = 2;

    /// <summary>Mensagem XML encapsulando o <see cref="PedidoCancelamentoNFe"/> para cancelamento.</summary>
    [XmlElement("MensagemXML", Namespace = Constants.Uris.Nfe)]
    public MensagemXml<PedidoCancelamentoNFe>? MensagemXml { get; set; }

    /// <summary>
    /// Cria uma nova instância de <see cref="CancelamentoNFeRequest"/> a partir de um <see cref="PedidoCancelamentoNFe"/>.
    /// </summary>
    /// <param name="value">Pedido de cancelamento de NFS-e.</param>
    public static CancelamentoNFeRequest FromPedidoCancelamentoNFe(PedidoCancelamentoNFe value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(new MensagemXml<PedidoCancelamentoNFe>(value));
    }

    /// <inheritdoc cref="FromPedidoCancelamentoNFe"/>
    public static explicit operator CancelamentoNFeRequest(PedidoCancelamentoNFe value) =>
        FromPedidoCancelamentoNFe(value);
}