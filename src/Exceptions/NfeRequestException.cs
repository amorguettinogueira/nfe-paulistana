using System.ComponentModel;
using System.Net;

namespace Nfe.Paulistana.Exceptions;

/// <summary>
/// Exceção lançada quando o webservice da NF-e Paulistana retorna um status HTTP de erro.
/// Expõe o código de status e os payloads de requisição/resposta como propriedades estruturadas.
/// </summary>
public sealed class NfeRequestException : Exception
{
    /// <summary>Código de status HTTP retornado pelo webservice.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>Payload XML da requisição enviada ao webservice.</summary>
    public string? RequestPayload { get; }

    /// <summary>Corpo da resposta retornada pelo webservice.</summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="NfeRequestException"/> com detalhes do erro HTTP.
    /// </summary>
    /// <param name="statusCode">Código de status HTTP retornado pelo webservice.</param>
    /// <param name="requestPayload">Payload XML da requisição enviada ao webservice.</param>
    /// <param name="responseBody">Corpo da resposta retornada pelo webservice.</param>
    internal NfeRequestException(HttpStatusCode statusCode, string? requestPayload, string? responseBody)
        : base($"Erro HTTP {(int)statusCode} ao chamar o webservice da NF-e Paulistana.")
    {
        StatusCode = statusCode;
        RequestPayload = requestPayload;
        ResponseBody = responseBody;
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="NfeRequestException"/> sem detalhes específicos.
    /// Usado para casos genéricos ou quando os detalhes não estão disponíveis.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NfeRequestException()
    {
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="NfeRequestException"/> com uma mensagem de erro personalizada.
    /// </summary>
    /// <param name="message">Mensagem de erro personalizada.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NfeRequestException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="NfeRequestException"/> com uma mensagem de erro personalizada e uma exceção interna.
    /// </summary>
    /// <param name="message">Mensagem de erro personalizada.</param>
    /// <param name="innerException">Exceção interna que causou o erro.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NfeRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}