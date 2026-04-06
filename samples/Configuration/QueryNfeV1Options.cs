using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para a ação de consulta de NF-e específica via serviço V01
/// (<see cref="Actions.QueryNfeV1Action"/>).
/// Chave User Secrets raiz: <c>ConsultaNfeV1</c>.
/// </summary>
internal sealed class QueryNfeV1Options
{
    /// <summary>
    /// Número sequencial da NF-e a consultar. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>ConsultaNfeV1:NumeroNotaFiscal</c>.
    /// </summary>
    [Required, Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.ConsultaNfeV1)}.{nameof(NumeroNotaFiscal)} deve ser maior que zero.")]
    public long NumeroNotaFiscal { get; init; }
}