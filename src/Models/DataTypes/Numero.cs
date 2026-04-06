using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para campos numéricos inteiros genéricos do XSD da NF-e Paulistana,
/// restrito ao intervalo <c>[1, 999.999.999.999]</c> com até 12 dígitos.
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpNumero</c>, linha 187.</para>
/// </remarks>
[Serializable]
public sealed class Numero : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em um número inteiro por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "O número deve ter no máximo 12 dígitos (máximo 999.999.999.999).";
    private const string InvalidMinValue = "O número deve ter pelo menos 1 dígito significativo (mínimo 1).";
    private const int MinValue = 1;
    private const long MaxValue = 999_999_999_999;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Numero()
    { }

    /// <summary>Inicializa o valor inteiro a partir de um <see cref="long"/>.</summary>
    /// <param name="value">Valor numérico. Deve estar entre <c>1</c> e <c>999.999.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public Numero(long value)
    {
        if (value < MinValue)
        {
            throw new ArgumentException(InvalidMinValue);
        }
        else if (value > MaxValue)
        {
            throw new ArgumentException(InvalidMaxValue);
        }

        Value = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Inicializa o valor inteiro a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">Número como string.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public Numero(string value)
        : this(long.TryParse(value.RemoveFormatting(), out long result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria um <see cref="Numero"/> a partir de um <see cref="long"/>.</summary>
    /// <param name="value">Valor numérico.</param>
    /// <returns>Nova instância de <see cref="Numero"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static Numero FromInt64(long value) => new(value);

    /// <summary>Cria um <see cref="Numero"/> a partir de uma string.</summary>
    /// <param name="value">Número como string.</param>
    /// <returns>Nova instância de <see cref="Numero"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Numero FromString(string value) => new(value);

    /// <inheritdoc cref="FromInt64"/>
    public static explicit operator Numero(long value) => FromInt64(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator Numero(string value) => FromString(value);
}