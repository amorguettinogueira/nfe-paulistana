using System.Globalization;

namespace Nfe.Paulistana.Extensions;

/// <summary>
/// <para>
/// Utilitários de extensão para manipulação de strings no contexto do domínio brasileiro.
/// </para>
/// <para>
/// Estes métodos suportam a necessidade do modelo de domínio de trabalhar com formatos
/// comuns de documentos brasileiros (CPF, CNPJ, CEP, etc.) que podem ser fornecidos
/// com formatação padrão (pontos, hífens, barras).
/// </para>
/// <para>
/// <strong>Nota Arquitetural:</strong> As extensões são mantidas aqui (em vez de em
/// classes base) para:
/// <list type="number">
/// <item>Manter separação de responsabilidades (formatação ≠ validação)</item>
/// <item>Habilitar reutilização em todo o domínio (não apenas em desserialização XML)</item>
/// <item>Seguir princípios SOLID (Segregação de Interface, Responsabilidade Única)</item>
/// <item>Manter utilitários coesos em um único local</item>
/// </list>
/// </para>
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Remove caracteres especiais comumente usados para formatar números de documentos no Brasil.
    /// </summary>
    /// <param name="value">String de entrada (pode ser nula ou vazia).</param>
    /// <returns>
    /// String contendo apenas os dígitos da entrada, na ordem original.
    /// Retorna string vazia se a entrada for nula, vazia ou não contiver dígitos.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Este método extrai apenas os caracteres numéricos de uma string, removendo
    /// toda formatação. É útil para normalizar documentos brasileiros antes de validação.
    /// </para>
    /// <para>
    /// Exemplos de formatação brasileira removida:
    /// <list type="bullet">
    /// <item>CPF: "123.456.789-01" → "12345678901"</item>
    /// <item>CNPJ: "12.345.678/0001-90" → "12345678000190"</item>
    /// <item>CEP: "12345-678" → "12345678"</item>
    /// <item>Telefone: "(11) 98765-4321" → "11987654321"</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string formatted = "123.456.789-01";
    /// string unformatted = formatted.RemoveFormatting(); // "12345678901"
    /// string nullable = null;
    /// string result = nullable.RemoveFormatting(); // ""
    /// </code>
    /// </example>
    public static string RemoveFormatting(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        ReadOnlySpan<char> source = value.AsSpan();
        Span<char> buffer = stackalloc char[source.Length];
        int pos = 0;

        foreach (char c in source)
        {
            if (char.IsAsciiDigit(c))
            {
                buffer[pos++] = c;
            }
        }

        return pos == 0 ? string.Empty : new string(buffer[..pos]);
    }

    /// <summary>
    /// Formata uma string usando <see cref="string.Format"/> com o provedor de formato
    /// padrão <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="value">String de entrada a ser formatada. Pode conter placeholders como {0}, {1}, etc.</param>
    /// <param name="args">Lista de argumentos a serem passados para <see cref="string.Format"/>.</param>
    /// <returns>String formatada com os argumentos fornecidos.</returns>
    /// <remarks>
    /// <para>
    /// Garante que todas as mensagens sejam formatadas igualmente em toda a implementação
    /// da biblioteca, usando a cultura invariante. Isto é crítico para mensagens de erro
    /// que possam ser traduzidas ou comparadas de forma consistente.
    /// </para>
    /// <para>
    /// O uso de <see cref="CultureInfo.InvariantCulture"/> assegura que:
    /// <list type="bullet">
    /// <item>Separadores decimais sejam sempre pontos (.)</item>
    /// <item>Datas tenham formato consistente</item>
    /// <item>Não haja variações baseadas em localização do usuário</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// string message = "O valor {0} está no intervalo {1} a {2}".Format(150, 100, 200);
    /// // Resultado: "O valor 150 está no intervalo 100 a 200"
    ///
    /// decimal value = 123.456m;
    /// string formatted = "Valor: {0:F2}".Format(value);
    /// // Resultado: "Valor: 123.46" (sempre com . como separador decimal)
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Se <paramref name="value"/> for nulo.</exception>
    /// <exception cref="FormatException">Se a string de formato for inválida.</exception>
    public static string Format(this string value, params object?[] args) =>
        string.Format(CultureInfo.InvariantCulture, value, args);
}