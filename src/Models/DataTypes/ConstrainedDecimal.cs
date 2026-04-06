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

        if (exponent > 0)
        {
            decimal result = 1m;
            for (int i = 0; i < exponent; i++)
            {
                result *= 10m;
            }
            return result;
        }

        decimal divisor = 1m;
        for (int i = 0; i < -exponent; i++)
        {
            divisor *= 10m;
        }
        return 1m / divisor;
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
}