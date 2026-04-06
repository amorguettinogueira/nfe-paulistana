using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de informações de lote ao webservice
/// da NF-e Paulistana v02.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaInformacoesLoteService(HttpClient httpClient) : IConsultaInformacoesLoteService
{
    private const string InvalidPayload = "Os dados do Pedido de Informações de Lote não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionConsultaInformacoesLote = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoInformacoesLote> SendAsync(
        PedidoInformacoesLote pedidoInformacoesLote,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoInformacoesLote);

        if (!pedidoInformacoesLote.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        var envelope = new SoapEnvelope<ConsultaInformacoesLoteRequest>((ConsultaInformacoesLoteRequest)pedidoInformacoesLote);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionConsultaInformacoesLote, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<ConsultaInformacoesLoteResponse> responseEnvelope = SoapClient.DeserializeEnvelope<ConsultaInformacoesLoteResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
