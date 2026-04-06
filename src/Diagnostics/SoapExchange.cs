using Nfe.Paulistana.Extensions;

namespace Nfe.Paulistana.Diagnostics;

/// <summary>
/// Representa um intercâmbio SOAP completo capturado durante uma chamada ao webservice da NF-e Paulistana.
/// Exposto via <see cref="SoapDiagnosticsHandler"/> e consumido pelo callback registrado em
/// <see cref="HttpClientBuilderExtensions.AddNfePaulistanaDiagnostics(Microsoft.Extensions.DependencyInjection.IHttpClientBuilder, Action{SoapExchange})"/>.
/// </summary>
/// <param name="SoapAction">
/// Valor do cabeçalho <c>SOAPAction</c> da requisição, sem aspas. Identifica a operação invocada
/// (ex.: <c>http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe</c>).
/// </param>
/// <param name="RequestXml">XML completo do envelope SOAP enviado ao webservice.</param>
/// <param name="ResponseXml">XML completo do envelope SOAP recebido do webservice.</param>
/// <param name="Elapsed">Tempo decorrido entre o envio da requisição e o recebimento da resposta completa.</param>
/// <param name="IsSuccess">
/// <see langword="true"/> se o status HTTP da resposta indicou sucesso (2xx);
/// <see langword="false"/> caso contrário. Quando <see langword="false"/>, uma
/// <see cref="Exceptions.NfeRequestException"/> será lançada pelo serviço após o retorno do handler.
/// </param>
public sealed record SoapExchange(
    string SoapAction,
    string RequestXml,
    string ResponseXml,
    TimeSpan Elapsed,
    bool IsSuccess);
