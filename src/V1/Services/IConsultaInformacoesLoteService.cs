using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de informações de lote
/// ao webservice da NF-e Paulistana.
/// </summary>
public interface IConsultaInformacoesLoteService : INfeService<PedidoInformacoesLote, RetornoInformacoesLote>
{
}
