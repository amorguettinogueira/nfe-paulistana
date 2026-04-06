using Nfe.Paulistana.Exceptions;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Cliente SOAP responsável por serializar envelopes e transmitir requisições ao webservice da NF-e Paulistana.
/// O <see cref="HttpClient"/> injetado é gerenciado pelo <see cref="IHttpClientFactory"/>.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> apontando para o endpoint SOAP do webservice.
/// </param>
internal sealed class SoapClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient ??
        throw new ArgumentNullException(nameof(httpClient));

    private static readonly XmlSerializerNamespaces _soapNamespaces =
        new([new("soap", "http://schemas.xmlsoap.org/soap/envelope/"),
             new("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
             new("xsd", "http://www.w3.org/2001/XMLSchema")]);

    /// <summary>
    /// Deserializa a resposta XML recebida do webservice em um <see cref="SoapEnvelope{TResponse}"/>.
    /// </summary>
    /// <typeparam name="TResponse">Tipo da resposta encapsulada no Body SOAP.</typeparam>
    /// <param name="xml">String XML da resposta SOAP.</param>
    /// <returns>Envelope SOAP deserializado com a resposta tipada.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="xml"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando a deserialização falha.</exception>
    public static SoapEnvelope<TResponse> DeserializeEnvelope<TResponse>(string xml) where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(xml);

        var serializer = new XmlSerializer(typeof(SoapEnvelope<TResponse>));
        using var reader = XmlReader.Create(new StringReader(xml));
        return (SoapEnvelope<TResponse>)(serializer.Deserialize(reader)
            ?? throw new InvalidOperationException("Falha ao deserializar o envelope SOAP da resposta."));
    }

    /// <summary>
    /// Serializa o envelope SOAP em uma string XML com codificação UTF-8 sem BOM.
    /// </summary>
    /// <typeparam name="TRequest">Tipo da requisição encapsulada no Body SOAP.</typeparam>
    /// <param name="soapEnvelopObject">Envelope SOAP a ser serializado.</param>
    /// <returns>String XML do envelope serializado.</returns>
    public static string SerializeEnvelope<TRequest>(SoapEnvelope<TRequest> soapEnvelopObject) where TRequest : class
    {
        var serializer = new XmlSerializer(typeof(SoapEnvelope<TRequest>));
        using var memoryStream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false) });
        xmlWriter.WriteStartDocument();
        serializer.Serialize(xmlWriter, soapEnvelopObject, _soapNamespaces);
        xmlWriter.Flush();
        return Encoding.UTF8.GetString(memoryStream.GetBuffer().AsSpan(0, (int)memoryStream.Length));
    }

    /// <summary>
    /// Envia o envelope SOAP ao endpoint configurado e retorna o corpo da resposta como string.
    /// </summary>
    /// <typeparam name="TRequest">Tipo da requisição encapsulada no Body SOAP.</typeparam>
    /// <param name="xmlPayload">Envelope SOAP contendo a requisição a ser transmitida.</param>
    /// <param name="soapAction">URI da SOAPAction utilizada por endpoints ASMX para rotear a requisição ao método correto.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    /// <returns>Corpo da resposta SOAP como string XML.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="xmlPayload"/> é nulo.</exception>
    /// <exception cref="NfeRequestException">Lançado quando o endpoint retorna status HTTP de erro.</exception>
    public async Task<string> SendRequestAsync<TRequest>(SoapEnvelope<TRequest> xmlPayload, string? soapAction = null, CancellationToken cancellationToken = default) where TRequest : class
    {
        ArgumentNullException.ThrowIfNull(xmlPayload);

        string payloadString = SerializeEnvelope(xmlPayload);

        var content = new StringContent(payloadString, Encoding.UTF8, "text/xml");

        using HttpRequestMessage request = new(HttpMethod.Post, _httpClient.BaseAddress)
        {
            Content = content
        };

        if (soapAction is not null)
        {
            request.Headers.Add("SOAPAction", $"\"{soapAction}\"");
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return !response.IsSuccessStatusCode
            ? throw new NfeRequestException(response.StatusCode, payloadString, responseBody)
            : responseBody;
    }
}