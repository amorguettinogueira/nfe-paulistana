using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de cancelamento de NFS-e ao webservice
/// da NF-e Paulistana v02.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class CancelamentoNFeService(HttpClient httpClient) : ICancelamentoNFeService
{
    private const string InvalidPayload = "Os dados do Pedido de Cancelamento de NFS-e não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionCancelamentoNFe = "http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoCancelamentoNFe> SendAsync(
        PedidoCancelamentoNFe pedidoCancelamentoNFe,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoCancelamentoNFe);

        if (!pedidoCancelamentoNFe.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        var envelope = new SoapEnvelope<CancelamentoNFeRequest>((CancelamentoNFeRequest)pedidoCancelamentoNFe);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionCancelamentoNFe, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<CancelamentoNFeResponse> responseEnvelope = SoapClient.DeserializeEnvelope<CancelamentoNFeResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
