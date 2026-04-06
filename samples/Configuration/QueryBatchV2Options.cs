using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para a ação de consulta de lote RPS via serviço V02
/// (<see cref="Actions.QueryBatchV2Action"/>).
/// Chave User Secrets raiz: <c>ConsultaLoteV2</c>.
/// </summary>
internal sealed class QueryBatchV2Options
{
    /// <summary>
    /// Número do lote RPS atribuído pela Prefeitura de São Paulo. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>ConsultaLoteV2:NumeroLote</c>.
    /// </summary>
    [Required, Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.ConsultaLoteV2)}.{nameof(NumeroLote)} deve ser maior que zero.")]
    public long NumeroLote { get; init; }
}