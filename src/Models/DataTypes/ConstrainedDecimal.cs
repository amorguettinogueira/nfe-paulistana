using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Classe base abstrata para Value Objects numéricos decimais do XSD da NF-e Paulistana.
/// Restringe o valor a um intervalo positivo configurável e o serializa sem parte fracionária
/// quando inteiro, ou com exatamente duas casas decimais quando fracionário.
/// </summary>
/// <remarks>
/// <para>
/// O limite padrão é 15 dígitos inteiros e 2 dígitos fracionários, resultando em valor máximo
/// de <c>999.999.999.999.999,99</c>. Subclasses podem redefinir esses limites via
/// <see cref="GetMaxLimit"/>.
/// </para>
/// <para>
/// Fonte: Manual NF-e Web Service v2.8.1, seção 3.4.4 —
/// <em>Regras de preenchimento dos campos</em>.
/// </para>
/// </remarks>
public abstract class ConstrainedDecimal : XmlSerializableDataType
{
    private const string InvalidValue = "O valor deve ser positivo e não deve ser maior do que {0}.";
    private const int IntegralMaxLength = 15;
    private const int FractionalMaxLength = 2;
    protected static readonly CultureInfo EnglishCulture = CultureInfo.CreateSpecificCulture("en-US");
    protected static readonly CultureInfo BrazilianCulture = CultureInfo.CreateSpecificCulture("pt-BR");

    /// <summary>
    /// Construtor sem parâmetros protegido exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected ConstrainedDecimal() { }

    /// <summary>
    /// Inicializa o Value Object validando que o valor é positivo e está dentro do limite
    /// configurado por <see cref="GetMaxLimit"/>.
    /// </summary>
    /// <param name="value">Valor decimal do campo.</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for negativo ou exceder o valor máximo permitido.
    /// </exception>
    protected ConstrainedDecimal(decimal value)
    {
        (_, int fractional, decimal maxValue) = GetDecimalLimits();

        if (!IsValidDecimal(value, maxValue))
        {
            throw new ArgumentException(InvalidValue.Format(maxValue.ToString($"N{fractional}", BrazilianCulture)));
        }

        Value = value.ToString(value % 1 == 0 ? "F0" : $"F{fractional}", EnglishCulture);
    }

    private static bool IsValidDecimal(decimal value, decimal maxValue) =>
        value >= 0 && value <= maxValue;

    private (int integral, int fractional, decimal maxValue) GetDecimalLimits()
    {
        (int integral, int fractional) = GetMaxLimit();

        decimal integerPart = DecimalPow10(integral);
        decimal fractionalPart = DecimalPow10(-fractional);

        return (integral, fractional, integerPart - fractionalPart);
    }

    private static decimal DecimalPow10(int exponent)
    {
        if (exponent == 0)
        {
            return 1m;
        }

        decimal factor = exponent > 0 ? 10m : 0.1m;
        decimal result = 1m;

        for (int i = 0; i < Math.Abs(exponent); i++)
        {
            result *= factor;
        }

        return result;
    }

    /// <summary>
    /// Retorna os limites de dígitos inteiros e fracionários para este campo.
    /// Subclasses devem sobrescrever para reduzir os limites padrão.
    /// </summary>
    /// <returns>
    /// Tupla <c>(integral, fractional)</c> onde <c>integral</c> é o número máximo de
    /// dígitos inteiros e <c>fractional</c> é o número de casas decimais.
    /// O padrão é <c>(15, 2)</c>.
    /// </returns>
    protected virtual (int integral, int fractional) GetMaxLimit() =>
        (IntegralMaxLength, FractionalMaxLength);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, invoca <paramref name="factory"/> para construir a instância.
    /// </summary>
    /// <remarks>
    /// Use este método nas subclasses para expor um <c>ParseIfPresent</c> público com tipagem forte.
    /// </remarks>
    /// <typeparam name="T">Tipo derivado de <see cref="ConstrainedDecimal"/>.</typeparam>
    /// <param name="value">Double de entrada, possivelmente nulo.</param>
    /// <param name="factory">Fábrica que cria a instância a partir de um double não nulo.</param>
    /// <returns>Instância de <typeparamref name="T"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="factory"/> for <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Propagada pela <paramref name="factory"/> se o valor não satisfizer as regras do tipo.</exception>
    protected static T? ParseIfPresent<T>(double? value, Func<double, T> factory) where T : ConstrainedDecimal
    {
        ArgumentNullException.ThrowIfNull(factory);
        return value.HasValue ? factory(value.Value) : null;
    }

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, invoca <paramref name="factory"/> para construir a instância.
    /// </summary>
    /// <remarks>
    /// Use este método nas subclasses para expor um <c>ParseIfPresent</c> público com tipagem forte.
    /// </remarks>
    /// <typeparam name="T">Tipo derivado de <see cref="ConstrainedDecimal"/>.</typeparam>
    /// <param name="value">Decimal de entrada, possivelmente nulo.</param>
    /// <param name="factory">Fábrica que cria a instância a partir de um decimal não nulo.</param>
    /// <returns>Instância de <typeparamref name="T"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="factory"/> for <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Propagada pela <paramref name="factory"/> se o valor não satisfizer as regras do tipo.</exception>
    protected static T? ParseIfPresent<T>(decimal? value, Func<decimal, T> factory) where T : ConstrainedDecimal
    {
        ArgumentNullException.ThrowIfNull(factory);
        return value.HasValue ? factory(value.Value) : null;
    }
}