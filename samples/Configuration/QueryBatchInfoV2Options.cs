namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// <para>Configurações para a ação <see cref="Actions.QueryBatchInfoV2Action"/>.</para>
/// <para>
/// Observação: esta classe é estruturalmente idêntica a
/// <see cref="QueryBatchInfoV1Options"/>, mantida separada
/// para mapear a seção de configuração correspondente e permitir
/// evolução independente entre versões.
/// </para>
/// </summary>
internal sealed class QueryBatchInfoV2Options
{
    /// <summary>
    /// Número do lote a consultar. Quando <c>null</c> não será aplicado filtro por lote (sem filtro).
    /// Use <c>null</c> para indicar que nenhuma restrição de número de lote deve ser aplicada.
    /// </summary>
    public long? NumeroLote { get; init; }
}