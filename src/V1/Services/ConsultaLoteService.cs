using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de lote ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaLoteService(HttpClient httpClient) : IConsultaLoteService
{
    private const string InvalidPayload = "Os dados do Pedido de Consulta de Lote não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionConsultaLote = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoConsulta> SendAsync(
        PedidoConsultaLote pedidoConsultaLote,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoConsultaLote);

        if (!pedidoConsultaLote.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        var envelope = new SoapEnvelope<ConsultaLoteRequest>((ConsultaLoteRequest)pedidoConsultaLote);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionConsultaLote, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<ConsultaLoteResponse> responseEnvelope = SoapClient.DeserializeEnvelope<ConsultaLoteResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
