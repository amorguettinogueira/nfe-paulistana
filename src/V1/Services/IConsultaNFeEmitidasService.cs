using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de consulta de NFS-e emitidas
/// ao webservice da NF-e Paulistana.
/// </summary>
public interface IConsultaNFeEmitidasService : INfeService<PedidoConsultaNFePeriodo, RetornoConsulta>
{
}
