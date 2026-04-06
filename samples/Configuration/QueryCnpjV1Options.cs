using System.ComponentModel.DataAnnotations;

namespace Nfe.Paulistana.Integration.Sample.Configuration;

/// <summary>
/// <para>Configurações para a ação <see cref="Actions.QueryCnpjV1Action"/>.</para>
/// <para>
/// Observação: esta classe é estruturalmente idêntica a
/// <see cref="QueryCnpjV2Options"/> (apenas a seção de configuração
/// difere). Mantemos tipos separados para refletir seções de
/// configuração distintas e para facilitar evolução independente.
/// Não unificar as classes sem avaliar impacto nas seções de configuração.
/// </para>
/// </summary>
internal sealed class QueryCnpjV1Options
{
    /// <summary>
    /// CNPJ do contribuinte a consultar na Prefeitura de São Paulo. Obrigatório.
    /// Formato: 14 dígitos numéricos sem separadores (ex.: <c>00000000000000</c>).
    /// Chave User Secrets: <c>ConsultaCnpjV1:CnpjAConsultar</c>.
    /// </summary>
    [Required(ErrorMessage = $"{nameof(AppSettings.ConsultaCnpjV1)}.{nameof(CnpjAConsultar)} é obrigatório.")]
    public string CnpjAConsultar { get; init; } = string.Empty;
}