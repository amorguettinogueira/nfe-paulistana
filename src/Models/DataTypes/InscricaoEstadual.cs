using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para Inscrição Estadual (IE), que aceita valores de 1 a 19 dígitos.
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpInscricaoEstadual</c>, linha 161.</para>
/// </remarks>
[Serializable]
public sealed class InscricaoEstadual : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em uma inscrição estadual por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "A inscrição estadual deve ter no máximo 19 dígitos (máximo 9.999.999.999.999.999.999).";
    private const string InvalidMinValue = "A inscrição estadual deve ter pelo menos 1 dígito significativo (mínimo 1).";
    private const int MinValue = 1;
    private const ulong MaxValue = 9_999_999_999_999_999_999;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InscricaoEstadual()
    { }

    /// <summary>Inicializa a Inscrição Estadual a partir de um número.</summary>
    /// <param name="value">Valor numérico da IE. Deve estar entre <c>1</c> e <c>9.999.999.999.999.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public InscricaoEstadual(ulong value)
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

    /// <summary>Inicializa a Inscrição Estadual a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">IE como string, com ou sem formatação.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public InscricaoEstadual(string value)
        : this(ulong.TryParse(value, out ulong result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria uma <see cref="InscricaoEstadual"/> a partir de uma string.</summary>
    /// <param name="value">IE como string.</param>
    /// <returns>Nova instância de <see cref="InscricaoEstadual"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static InscricaoEstadual FromString(string value) => new(value);

    /// <summary>Cria uma <see cref="InscricaoEstadual"/> a partir de um número.</summary>
    /// <param name="value">Valor numérico da IE.</param>
    /// <returns>Nova instância de <see cref="InscricaoEstadual"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static InscricaoEstadual FromUInt64(ulong value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator InscricaoEstadual(string value) => FromString(value);

    /// <inheritdoc cref="FromUInt64"/>
    public static explicit operator InscricaoEstadual(ulong value) => FromUInt64(value);
}