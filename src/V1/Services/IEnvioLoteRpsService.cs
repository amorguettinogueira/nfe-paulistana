using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de envio em lote de RPS ao webservice
/// da NF-e Paulistana, tanto em modo de produção quanto em modo de teste.
/// </summary>
public interface IEnvioLoteRpsService : INfeLoteRpsService<PedidoEnvioLote, RetornoEnvioLoteRps>
{
}
