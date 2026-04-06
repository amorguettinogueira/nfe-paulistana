using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de envio de RPS unitário ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class EnvioRpsService(HttpClient httpClient) : IEnvioRpsService
{
    private const string InvalidPayload = "Os dados do Pedido de Envio de RPS não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionEnvioRps = "http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoEnvioRps> SendAsync(PedidoEnvio pedidoEnvio, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoEnvio);

        if (!pedidoEnvio.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        var envelope = new SoapEnvelope<EnvioRpsRequest>((EnvioRpsRequest)pedidoEnvio);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionEnvioRps, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<EnvioRpsResponse> responseEnvelope = SoapClient.DeserializeEnvelope<EnvioRpsResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
