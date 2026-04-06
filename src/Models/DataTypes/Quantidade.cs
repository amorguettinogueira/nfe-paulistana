using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para o campo de quantidade no cabeçalho do lote (<c>QtdRPS</c>),
/// restrito ao intervalo <c>[1, 999.999.999.999.999]</c> com até 15 dígitos.
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpQuantidade</c>.</para>
/// </remarks>
[Serializable]
public sealed class Quantidade : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em uma quantidade por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "O número deve ter no máximo 15 dígitos (máximo 999.999.999.999.999).";
    private const string InvalidMinValue = "O número deve ter pelo menos 1 dígito significativo (mínimo 1).";
    private const int MinValue = 1;
    private const long MaxValue = 999_999_999_999_999;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Quantidade()
    { }

    /// <summary>Inicializa a quantidade a partir de um <see cref="long"/>.</summary>
    /// <param name="value">Quantidade. Deve estar entre <c>1</c> e <c>999.999.999.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public Quantidade(long value)
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

    /// <summary>Inicializa a quantidade a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">Quantidade como string.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public Quantidade(string value)
        : this(long.TryParse(value.RemoveFormatting(), out long result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria uma <see cref="Quantidade"/> a partir de um <see cref="long"/>.</summary>
    /// <param name="value">Quantidade.</param>
    /// <returns>Nova instância de <see cref="Quantidade"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static Quantidade FromInt64(long value) => new(value);

    /// <summary>Cria uma <see cref="Quantidade"/> a partir de uma string.</summary>
    /// <param name="value">Quantidade como string.</param>
    /// <returns>Nova instância de <see cref="Quantidade"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Quantidade FromString(string value) => new(value);

    /// <inheritdoc cref="FromInt64"/>
    public static explicit operator Quantidade(long value) => FromInt64(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator Quantidade(string value) => FromString(value);
}