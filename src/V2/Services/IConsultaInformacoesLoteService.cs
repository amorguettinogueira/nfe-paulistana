using Nfe.Paulistana.Abstractions;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Contrato do serviço responsável por enviar pedidos de informações de lote
/// ao webservice da NF-e Paulistana v02.
/// </summary>
public interface IConsultaInformacoesLoteService : INfeService<PedidoInformacoesLote, RetornoInformacoesLote>
{
}
