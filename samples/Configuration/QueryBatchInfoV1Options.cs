namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// <para>Configurações para a ação <see cref="Actions.QueryBatchInfoV1Action"/>.</para>
/// <para>
/// Observação: esta classe é estruturalmente idêntica a
/// <see cref="QueryBatchInfoV2Options"/>. Mantemos tipos
/// separados porque cada versão lê uma seção diferente nos
/// User Secrets / configuração e podem divergir no futuro.
/// </para>
/// </summary>
internal sealed class QueryBatchInfoV1Options
{
    /// <summary>
    /// Número do lote a consultar. Quando <c>null</c> não será aplicado filtro por lote (sem filtro).
    /// Use <c>null</c> para indicar que nenhuma restrição de número de lote deve ser aplicada.
    /// </summary>
    public long? NumeroLote { get; init; }
}