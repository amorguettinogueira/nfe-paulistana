using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de NFS-e ao webservice
/// da NF-e Paulistana v02.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaNFeService(HttpClient httpClient) : IConsultaNFeService
{
    private const string InvalidPayload = "Os dados do Pedido de Consulta de NFS-e não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionConsultaNFe = "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoConsulta> SendAsync(
        PedidoConsultaNFe pedidoConsultaNFe,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoConsultaNFe);

        if (!pedidoConsultaNFe.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        var envelope = new SoapEnvelope<ConsultaNFeRequest>((ConsultaNFeRequest)pedidoConsultaNFe);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionConsultaNFe, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<ConsultaNFeResponse> responseEnvelope = SoapClient.DeserializeEnvelope<ConsultaNFeResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
