using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para a ação de cancelamento de NF-e via serviço V01
/// (<see cref="Actions.CancelNfeV1Action"/>).
/// Chave User Secrets raiz: <c>CancelamentoNfeV1</c>.
/// </summary>
internal sealed class CancelNfeV1Options
{
    /// <summary>
    /// Número sequencial da NF-e a ser cancelada. Obrigatório.
    /// Deve ser um inteiro positivo (range: 1 a <see cref="long.MaxValue"/>).
    /// Chave User Secrets: <c>CancelamentoNfeV1:NumeroNFeParaCancelar</c>.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = $"{nameof(AppSettings.CancelamentoNfeV1)}.{nameof(NumeroNFeParaCancelar)} deve ser maior que zero.")]
    public long NumeroNFeParaCancelar { get; init; }
}