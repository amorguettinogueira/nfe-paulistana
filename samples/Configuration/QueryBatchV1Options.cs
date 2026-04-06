using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para a ação de consulta de lote RPS via serviço V01
/// (<see cref="Actions.QueryBatchV1Action"/>).
/// Chave User Secrets raiz: <c>ConsultaLoteV1</c>.
/// </summary>
internal sealed class QueryBatchV1Options
{
    /// <summary>
    /// Número do lote RPS atribuído pela Prefeitura de São Paulo. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>ConsultaLoteV1:NumeroLote</c>.
    /// </summary>
    [Required, Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.ConsultaLoteV1)}.{nameof(NumeroLote)} deve ser maior que zero.")]
    public long NumeroLote { get; init; }
}