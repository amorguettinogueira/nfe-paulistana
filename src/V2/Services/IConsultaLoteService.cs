using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de consulta de lote v02
/// ao webservice da NF-e Paulistana.
/// </summary>
public interface IConsultaLoteService : INfeService<PedidoConsultaLote, RetornoConsulta>
{
}
