using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para Código de Endereçamento Postal (CEP) brasileiro,
/// restrito ao intervalo <c>[1.000.000, 99.999.999]</c> (7 a 8 dígitos significativos).
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpCEP</c>, linha 63.</para>
/// </remarks>
[Serializable]
public sealed class Cep : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em um CEP válido por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "O CEP deve ter no máximo 8 dígitos (máximo 99.999-999).";
    private const string InvalidMinValue = "O CEP deve ter pelo menos 7 dígitos significativos (mínimo 01.000-000).";
    private const int MinValue = 1_000_000;
    private const int MaxValue = 99_999_999;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Cep()
    { }

    /// <summary>Inicializa o CEP a partir de um número inteiro.</summary>
    /// <param name="value">Código numérico do CEP. Deve estar entre <c>1.000.000</c> e <c>99.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public Cep(int value)
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

    /// <summary>Inicializa o CEP a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">CEP como string, com ou sem formatação (ex: <c>"01310-100"</c>).</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public Cep(string value)
        : this(int.TryParse(value.RemoveFormatting(), out int result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria um <see cref="Cep"/> a partir de uma string.</summary>
    /// <param name="value">CEP como string, com ou sem formatação.</param>
    /// <returns>Nova instância de <see cref="Cep"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Cep FromString(string value) => new(value);

    /// <summary>Cria um <see cref="Cep"/> a partir de um número inteiro.</summary>
    /// <param name="value">Código numérico do CEP.</param>
    /// <returns>Nova instância de <see cref="Cep"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static Cep FromInt32(int value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator Cep(string value) => FromString(value);

    /// <inheritdoc cref="FromInt32"/>
    public static explicit operator Cep(int value) => FromInt32(value);
}