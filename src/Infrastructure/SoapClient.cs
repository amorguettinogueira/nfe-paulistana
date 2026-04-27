using Microsoft.IO;
using Nfe.Paulistana.Exceptions;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Cliente SOAP responsável por serializar envelopes e transmitir requisições ao webservice da NF-e Paulistana.
/// O <see cref="HttpClient"/> injetado é gerenciado pelo <see cref="IHttpClientFactory"/>.
/// </summary>
/// <remarks>
/// <para>
/// Instâncias de <see cref="XmlSerializer"/> são armazenadas em um cache estático do tipo
/// <see cref="ConcurrentDictionary{TKey,TValue}"/>, keyed pelo <see cref="Type"/> concreto do envelope,
/// eliminando três fontes de custo presentes na construção por chamada:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <b>Alocação por chamada:</b> cada <c>new XmlSerializer(type)</c> aloca um novo objeto no heap
///     mesmo quando o assembly de serialização já foi compilado pelo BCL. O cache garante que apenas
///     uma instância por tipo existe durante todo o tempo de vida do processo.
///   </description></item>
///   <item><description>
///     <b>Contenção de lock em alta concorrência:</b> o cache interno do BCL usa um <c>Hashtable</c>
///     protegido por lock. Em cenários com muitas threads simultâneas, cada construção disputa esse
///     lock. O <see cref="ConcurrentDictionary{TKey,TValue}"/> utiliza leituras lock-free em caminhos
///     quentes, reduzindo a contenção.
///   </description></item>
///   <item><description>
///     <b>Corrida no cold start:</b> múltiplas threads que chegam simultaneamente antes da primeira
///     entrada no cache do BCL podem cada uma disparar uma compilação independente do assembly de
///     serialização. O <see cref="ConcurrentDictionary{TKey,TValue}.GetOrAdd"/> limita isso a no máximo
///     uma construção extra descartada, estabilizando imediatamente após.
///   </description></item>
/// </list>
/// </remarks>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> apontando para o endpoint SOAP do webservice.
/// </param>
internal sealed class SoapClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient ??
        throw new ArgumentNullException(nameof(httpClient));

    private static readonly ConcurrentDictionary<Type, XmlSerializer> _serializerCache = new();

    private static readonly XmlSerializerNamespaces _soapNamespaces =
        new([new("soap", "http://schemas.xmlsoap.org/soap/envelope/"),
             new("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
             new("xsd", "http://www.w3.org/2001/XMLSchema")]);

    /// <summary>
    /// Retorna a instância de <see cref="XmlSerializer"/> para o tipo <typeparamref name="T"/> do cache estático,
    /// criando-a na primeira chamada. Instâncias de <see cref="XmlSerializer"/> são thread-safe para
    /// <c>Serialize</c> e <c>Deserialize</c> e podem ser compartilhadas livremente entre threads.
    /// </summary>
    /// <typeparam name="T">Tipo concreto para o qual o serializador será obtido ou criado.</typeparam>
    /// <returns>Instância de <see cref="XmlSerializer"/> reusável para <typeparamref name="T"/>.</returns>
    private static XmlSerializer GetSerializer<T>() =>
        _serializerCache.GetOrAdd(typeof(T), static t => new XmlSerializer(t));

    /// <summary>
    /// Deserializa a resposta XML recebida do webservice a partir de um <see cref="Stream"/> em um
    /// <see cref="SoapEnvelope{TResponse}"/>. O stream é lido incrementalmente, sem materializar
    /// a resposta completa como string.
    /// </summary>
    /// <typeparam name="TResponse">Tipo da resposta encapsulada no Body SOAP.</typeparam>
    /// <param name="stream">Stream da resposta SOAP.</param>
    /// <returns>Envelope SOAP deserializado com a resposta tipada.</returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="stream"/> é nulo.</exception>
    /// <exception cref="InvalidOperationException">Lançado quando a deserialização falha.</exception>
    public static SoapEnvelope<TResponse> DeserializeEnvelope<TResponse>(Stream stream) where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(stream);

        XmlSerializer serializer = GetSerializer<SoapEnvelope<TResponse>>();
        using var reader = XmlReader.Create(stream);
        return (SoapEnvelope<TResponse>)(serializer.Deserialize(reader)
            ?? throw new InvalidOperationException("Falha ao deserializar o envelope SOAP da resposta."));
    }

    /// <summary>
    /// Serializa o envelope SOAP em um <see cref="MemoryStream"/> poolado com codificação UTF-8 sem BOM.
    /// O stream é retornado posicionado em zero e deve ser descartado pelo chamador.
    /// </summary>
    /// <typeparam name="TRequest">Tipo da requisição encapsulada no Body SOAP.</typeparam>
    /// <param name="soapEnvelopObject">Envelope SOAP a ser serializado.</param>
    /// <returns><see cref="MemoryStream"/> poolado posicionado em zero com o XML serializado.</returns>
    public static MemoryStream SerializeEnvelope<TRequest>(SoapEnvelope<TRequest> soapEnvelopObject) where TRequest : class
    {
        XmlSerializer serializer = GetSerializer<SoapEnvelope<TRequest>>();
        RecyclableMemoryStream memoryStream = StreamManager.Instance.GetStream();
        using var xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false) });
        xmlWriter.WriteStartDocument();
        serializer.Serialize(xmlWriter, soapEnvelopObject, _soapNamespaces);
        xmlWriter.Flush();
        _ = memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// Envia o envelope SOAP ao endpoint configurado e retorna a <see cref="HttpResponseMessage"/> de sucesso.
    /// O corpo da resposta é exposto como stream para deserialização incremental pelo chamador.
    /// </summary>
    /// <typeparam name="TRequest">Tipo da requisição encapsulada no Body SOAP.</typeparam>
    /// <param name="xmlPayload">Envelope SOAP contendo a requisição a ser transmitida.</param>
    /// <param name="soapAction">URI da SOAPAction utilizada por endpoints ASMX para rotear a requisição ao método correto.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    /// <returns>
    /// <see cref="HttpResponseMessage"/> com status de sucesso. O chamador é responsável por descartar a instância
    /// após a leitura do corpo da resposta.
    /// </returns>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="xmlPayload"/> é nulo.</exception>
    /// <exception cref="NfeRequestException">Lançado quando o endpoint retorna status HTTP de erro.</exception>
    public async Task<HttpResponseMessage> SendRequestAsync<TRequest>(SoapEnvelope<TRequest> xmlPayload, string? soapAction = null, CancellationToken cancellationToken = default) where TRequest : class
    {
        ArgumentNullException.ThrowIfNull(xmlPayload);

        MemoryStream requestStream = SerializeEnvelope(xmlPayload);
        await using (requestStream.ConfigureAwait(false))
        {
            var content = new StreamContent(requestStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/xml") { CharSet = "utf-8" };

            using HttpRequestMessage request = new(HttpMethod.Post, _httpClient.BaseAddress)
            {
                Content = content
            };

            if (soapAction is not null)
            {
                request.Headers.Add("SOAPAction", $"\"{soapAction}\"");
            }

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                using (response)
                {
                    // Lê corpo do erro; requestStream ainda válido (MemoryStream é seekable)
                    string payloadString = Encoding.UTF8.GetString(requestStream.ToArray());
                    string responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    throw new NfeRequestException(response.StatusCode, payloadString, responseBody);
                }
            }

            return response;
        }
    }
}