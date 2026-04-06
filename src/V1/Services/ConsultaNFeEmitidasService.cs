using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de NFS-e emitidas ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaNFeEmitidasService(HttpClient httpClient) : IConsultaNFeEmitidasService
{
    private const string InvalidPayload = "Os dados do Pedido de Consulta de NFS-e Emitidas não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionConsultaNFeEmitidas = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeEmitidas";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoConsulta> SendAsync(
        PedidoConsultaNFePeriodo pedidoConsultaNFePeriodo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoConsultaNFePeriodo);

        if (!pedidoConsultaNFePeriodo.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        var envelope = new SoapEnvelope<ConsultaNFeEmitidasRequest>((ConsultaNFeEmitidasRequest)pedidoConsultaNFePeriodo);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionConsultaNFeEmitidas, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<ConsultaNFeEmitidasResponse> responseEnvelope = SoapClient.DeserializeEnvelope<ConsultaNFeEmitidasResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
