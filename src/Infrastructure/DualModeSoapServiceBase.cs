using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Classe base abstrata para serviços SOAP da NF-e Paulistana que suportam dois modos de
/// operação: produção e teste, cada um com seu próprio par de envelopes SOAP distintos.
/// </summary>
/// <typeparam name="TRequest">Tipo do pedido de domínio, que implementa <see cref="IXmlValidatableSchema"/>.</typeparam>
/// <typeparam name="TEnvelopeRequest">Tipo do elemento raiz da requisição SOAP de produção.</typeparam>
/// <typeparam name="TEnvelopeResponse">Tipo do elemento raiz da resposta SOAP de produção.</typeparam>
/// <typeparam name="TTesteRequest">Tipo do elemento raiz da requisição SOAP de teste.</typeparam>
/// <typeparam name="TTesteResponse">Tipo do elemento raiz da resposta SOAP de teste.</typeparam>
/// <typeparam name="TResponse">Tipo do retorno de domínio extraído de ambos os modos.</typeparam>
internal abstract class DualModeSoapServiceBase<TRequest, TEnvelopeRequest, TEnvelopeResponse, TTesteRequest, TTesteResponse, TResponse>
    where TRequest : class, IXmlValidatableSchema
    where TEnvelopeRequest : class
    where TEnvelopeResponse : class
    where TTesteRequest : class
    where TTesteResponse : class
    where TResponse : class
{
    private const string MensagemRespostaVazia = "O webservice retornou uma resposta vazia ou inválida.";

    private readonly SoapClient _soapClient;
    private readonly string _soapAction;
    private readonly string _soapActionTeste;
    private readonly string _mensagemPayloadInvalido;

    /// <summary>
    /// Inicializa uma nova instância de
    /// <see cref="DualModeSoapServiceBase{TRequest, TEnvelopeRequest, TEnvelopeResponse, TTesteRequest, TTesteResponse, TResponse}"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
    /// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
    /// </param>
    /// <param name="soapAction">URI da SOAPAction para o modo de produção.</param>
    /// <param name="soapActionTeste">URI da SOAPAction para o modo de teste.</param>
    /// <param name="mensagemPayloadInvalido">
    /// Template de mensagem de erro de validação XSD. Deve conter um marcador <c>{0}</c>
    /// para receber os detalhes do erro.
    /// </param>
    protected DualModeSoapServiceBase(
        HttpClient httpClient,
        string soapAction,
        string soapActionTeste,
        string mensagemPayloadInvalido)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _soapClient = new SoapClient(httpClient);
        _soapAction = soapAction;
        _soapActionTeste = soapActionTeste;
        _mensagemPayloadInvalido = mensagemPayloadInvalido;
    }

    /// <summary>Cria o envelope de requisição SOAP para o modo de produção.</summary>
    protected abstract TEnvelopeRequest CreateEnvelope(TRequest request);

    /// <summary>Extrai o retorno de domínio a partir do elemento raiz da resposta SOAP de produção.</summary>
    protected abstract TResponse? ExtractPayload(TEnvelopeResponse response);

    /// <summary>Cria o envelope de requisição SOAP para o modo de teste.</summary>
    protected abstract TTesteRequest CreateTesteEnvelope(TRequest request);

    /// <summary>Extrai o retorno de domínio a partir do elemento raiz da resposta SOAP de teste.</summary>
    protected abstract TResponse? ExtractTestePayload(TTesteResponse response);

    /// <summary>
    /// Valida o pedido contra o XSD e envia ao webservice no modo de produção ou de teste,
    /// conforme indicado por <paramref name="modoTeste"/>.
    /// </summary>
    /// <param name="request">Pedido de domínio a ser enviado.</param>
    /// <param name="modoTeste">
    /// Quando <see langword="true"/>, o pedido é transmitido usando o envelope e a SOAPAction de teste,
    /// sem impacto nos registros fiscais.
    /// </param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Retorno de domínio <typeparamref name="TResponse"/> extraído da resposta SOAP.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="request"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">
    /// Lançado quando a validação XSD falha ou a resposta do webservice é vazia ou inválida.
    /// </exception>
    protected async Task<TResponse> SendAsync(TRequest request, bool modoTeste, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(_mensagemPayloadInvalido.Format(error));
        }

        return modoTeste
            ? await SendTesteAsync(request, cancellationToken).ConfigureAwait(false)
            : await SendProducaoAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResponse> SendProducaoAsync(TRequest request, CancellationToken cancellationToken)
    {
        var envelope = new SoapEnvelope<TEnvelopeRequest>(CreateEnvelope(request));
        string responseXml = await _soapClient.SendRequestAsync(envelope, _soapAction, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<TEnvelopeResponse> responseEnvelope = SoapClient.DeserializeEnvelope<TEnvelopeResponse>(responseXml);

        return responseEnvelope.Body?.Request is { } body
            ? ExtractPayload(body) ?? throw new InvalidOperationException(MensagemRespostaVazia)
            : throw new InvalidOperationException(MensagemRespostaVazia);
    }

    private async Task<TResponse> SendTesteAsync(TRequest request, CancellationToken cancellationToken)
    {
        var envelope = new SoapEnvelope<TTesteRequest>(CreateTesteEnvelope(request));
        string responseXml = await _soapClient.SendRequestAsync(envelope, _soapActionTeste, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<TTesteResponse> responseEnvelope = SoapClient.DeserializeEnvelope<TTesteResponse>(responseXml);

        return responseEnvelope.Body?.Request is { } body
            ? ExtractTestePayload(body) ?? throw new InvalidOperationException(MensagemRespostaVazia)
            : throw new InvalidOperationException(MensagemRespostaVazia);
    }
}
