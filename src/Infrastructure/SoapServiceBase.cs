using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Classe base abstrata para serviços SOAP da NF-e Paulistana que seguem o padrão
/// de validação XSD e envio/recebimento de envelopes SOAP tipados.
/// </summary>
/// <typeparam name="TRequest">Tipo do pedido de domínio, que implementa <see cref="IXmlValidatableSchema"/>.</typeparam>
/// <typeparam name="TEnvelopeRequest">Tipo do elemento raiz da requisição SOAP.</typeparam>
/// <typeparam name="TEnvelopeResponse">Tipo do elemento raiz da resposta SOAP.</typeparam>
/// <typeparam name="TResponse">Tipo do retorno de domínio extraído da resposta SOAP.</typeparam>
internal abstract class SoapServiceBase<TRequest, TEnvelopeRequest, TEnvelopeResponse, TResponse>
    where TRequest : class, IXmlValidatableSchema
    where TEnvelopeRequest : class
    where TEnvelopeResponse : class
    where TResponse : class
{
    private const string MensagemRespostaVazia = "O webservice retornou uma resposta vazia ou inválida.";

    private readonly SoapClient _soapClient;
    private readonly string _soapAction;
    private readonly string _mensagemPayloadInvalido;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="SoapServiceBase{TRequest, TEnvelopeRequest, TEnvelopeResponse, TResponse}"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
    /// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
    /// </param>
    /// <param name="soapAction">URI da SOAPAction específica da operação.</param>
    /// <param name="mensagemPayloadInvalido">
    /// Template de mensagem de erro de validação XSD. Deve conter um marcador <c>{0}</c>
    /// para receber os detalhes do erro.
    /// </param>
    protected SoapServiceBase(HttpClient httpClient, string soapAction, string mensagemPayloadInvalido)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _soapClient = new SoapClient(httpClient);
        _soapAction = soapAction;
        _mensagemPayloadInvalido = mensagemPayloadInvalido;
    }

    /// <summary>
    /// Cria o envelope de requisição SOAP a partir do pedido de domínio.
    /// </summary>
    /// <param name="request">Pedido de domínio validado.</param>
    /// <returns>Instância de <typeparamref name="TEnvelopeRequest"/> pronta para envio.</returns>
    protected abstract TEnvelopeRequest CreateEnvelope(TRequest request);

    /// <summary>
    /// Extrai o retorno de domínio <typeparamref name="TResponse"/> a partir do elemento raiz da resposta SOAP.
    /// </summary>
    /// <param name="response">Elemento raiz da resposta SOAP deserializada.</param>
    /// <returns>Instância de <typeparamref name="TResponse"/>, ou <see langword="null"/> se ausente.</returns>
    protected abstract TResponse? ExtractPayload(TEnvelopeResponse response);

    /// <summary>
    /// Valida o pedido contra o XSD, serializa, envia ao webservice e deserializa a resposta.
    /// </summary>
    /// <param name="request">Pedido de domínio a ser enviado.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Retorno de domínio <typeparamref name="TResponse"/> extraído da resposta SOAP.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="request"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">
    /// Lançado quando a validação XSD falha ou a resposta do webservice é vazia ou inválida.
    /// </exception>
    protected async Task<TResponse> SendAsync(TRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(_mensagemPayloadInvalido.Format(error));
        }

        var envelope = new SoapEnvelope<TEnvelopeRequest>(CreateEnvelope(request));
        string responseXml = await _soapClient.SendRequestAsync(envelope, _soapAction, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<TEnvelopeResponse> responseEnvelope = SoapClient.DeserializeEnvelope<TEnvelopeResponse>(responseXml);

        return responseEnvelope.Body?.Request is { } body
            ? ExtractPayload(body) ?? throw new InvalidOperationException(MensagemRespostaVazia)
            : throw new InvalidOperationException(MensagemRespostaVazia);
    }
}
