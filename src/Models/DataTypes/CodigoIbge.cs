using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para o código municipal do IBGE, com 7 dígitos significativos.
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpCidade</c>, linha 66.</para>
/// </remarks>
[Serializable]
public sealed class CodigoIbge : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em um código do município do IBGE por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "O código do município do IBGE deve ter no máximo 7 dígitos (máximo 9.999.999).";
    private const string InvalidMinValue = "O código do município do IBGE deve ter pelo menos 7 dígitos significativos (mínimo 1.000.000).";
    private const int MinValue = 1_000_000;
    private const int MaxValue = 9_999_999;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoIbge()
    { }

    /// <summary>Inicializa o código IBGE a partir de um número inteiro.</summary>
    /// <param name="value">Código numérico do município. Deve estar entre <c>1.000.000</c> e <c>99.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public CodigoIbge(int value)
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

    /// <summary>Inicializa o código IBGE a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">Código do município como string, com ou sem formatação (ex: <c>"3550308"</c>).</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public CodigoIbge(string value)
        : this(int.TryParse(value.RemoveFormatting(), out int result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria um <see cref="CodigoIbge"/> a partir de uma string.</summary>
    /// <param name="value">Código do município como string.</param>
    /// <returns>Nova instância de <see cref="CodigoIbge"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoIbge FromString(string value) => new(value);

    /// <summary>Cria um <see cref="CodigoIbge"/> a partir de um número inteiro.</summary>
    /// <param name="value">Código numérico do município.</param>
    /// <returns>Nova instância de <see cref="CodigoIbge"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static CodigoIbge FromInt32(int value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CodigoIbge(string value) => FromString(value);

    /// <inheritdoc cref="FromInt32"/>
    public static explicit operator CodigoIbge(int value) => FromInt32(value);
}