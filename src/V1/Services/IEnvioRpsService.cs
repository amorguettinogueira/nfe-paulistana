using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de envio de RPS unitário ao webservice
/// da NF-e Paulistana.
/// </summary>
public interface IEnvioRpsService : INfeService<PedidoEnvio, RetornoEnvioRps>
{
}
