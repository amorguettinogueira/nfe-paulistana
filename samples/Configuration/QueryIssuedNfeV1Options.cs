using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// Parâmetros para a ação de consulta de NF-e emitidas via serviço V01
/// (<see cref="Actions.QueryIssuedNfeV1Action"/>).
/// Chave User Secrets raiz: <c>ConsultaNfeEmitidasV1</c>.
/// </summary>
internal sealed class QueryIssuedNfeV1Options
{
    /// <summary>
    /// Número da página de resultados (paginação). Obrigatório.
    /// Deve ser pelo menos 1 (range: 1 a <see cref="int.MaxValue"/>).
    /// Chave User Secrets: <c>ConsultaNfeEmitidasV1:NumeroPagina</c>.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = $"{nameof(AppSettings.ConsultaNfeEmitidasV1)}.{nameof(NumeroPagina)} deve ser pelo menos 1.")]
    public long NumeroPagina { get; init; }

    /// <summary>
    /// Quantidade de dias a subtrair de <see cref="DateTime.Today"/> para definir
    /// o início do período de consulta; o fim é sempre <c>DateTime.Today</c>.
    /// Range válido: 0 (somente hoje) a 31 dias.
    /// Chave User Secrets: <c>ConsultaNfeEmitidasV1:ConsultaNDiasAtras</c>.
    /// </summary>
    [Range(0, 31, ErrorMessage = $"{nameof(AppSettings.ConsultaNfeEmitidasV1)}.{nameof(ConsultaNDiasAtras)} inválido.")]
    public int ConsultaNDiasAtras { get; init; }
}