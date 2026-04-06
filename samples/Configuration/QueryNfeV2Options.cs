using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para a ação de consulta de NF-e específica via serviço V02
/// (<see cref="Actions.QueryNfeV2Action"/>).
/// Chave User Secrets raiz: <c>ConsultaNfeV2</c>.
/// </summary>
internal sealed class QueryNfeV2Options
{
    /// <summary>
    /// Número sequencial da NF-e a consultar. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>ConsultaNfeV2:NumeroNotaFiscal</c>.
    /// </summary>
    [Required, Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.ConsultaNfeV2)}.{nameof(NumeroNotaFiscal)} deve ser maior que zero.")]
    public long NumeroNotaFiscal { get; init; }
}