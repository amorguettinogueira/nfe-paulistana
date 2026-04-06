using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de consulta de lote
/// ao webservice da NF-e Paulistana.
/// </summary>
public interface IConsultaLoteService : INfeService<PedidoConsultaLote, RetornoConsulta>
{
}
