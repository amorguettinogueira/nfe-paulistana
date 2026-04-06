using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object para Inscrição Municipal (IM) v02, armazenada como string numérica
/// de 1 a 12 dígitos, conforme exigido pelo XSD v02 da NF-e Paulistana.
/// </summary>
/// <remarks>
/// <para>
/// Diferente da v01 (que exige exatamente 8 dígitos com zero-padding),
/// a v02 aceita de 1 a 12 dígitos sem padding obrigatório.
/// Utiliza <c>long</c> como tipo interno para suportar valores de até 12 dígitos.
/// </para>
/// <para>Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpInscricaoMunicipal</c>.</para>
/// </remarks>
[Serializable]
public sealed class InscricaoMunicipal : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em uma inscrição municipal por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "A inscrição municipal deve ter no máximo 12 dígitos (máximo 999.999.999.999).";
    private const string InvalidMinValue = "A inscrição municipal deve ter pelo menos 1 dígito significativo (mínimo 1).";
    private const long MinValue = 1;
    private const long MaxValue = 999_999_999_999;
    private const int InscricaoMunicipalFormattingLength = 12;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InscricaoMunicipal()
    { }

    /// <summary>Inicializa a Inscrição Municipal a partir de um número inteiro longo.</summary>
    /// <param name="value">Valor numérico da IM. Deve estar entre <c>1</c> e <c>999.999.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public InscricaoMunicipal(long value)
    {
        if (value < MinValue)
        {
            throw new ArgumentException(InvalidMinValue);
        }
        else if (value > MaxValue)
        {
            throw new ArgumentException(InvalidMaxValue);
        }

        Value = value.ToString(CultureInfo.InvariantCulture).PadLeft(InscricaoMunicipalFormattingLength, '0');
    }

    /// <summary>Inicializa a Inscrição Municipal a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">IM como string, com ou sem formatação.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public InscricaoMunicipal(string value)
        : this(long.TryParse(value.RemoveFormatting(), out long result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria uma <see cref="InscricaoMunicipal"/> a partir de uma string.</summary>
    /// <param name="value">IM como string.</param>
    /// <returns>Nova instância de <see cref="InscricaoMunicipal"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static InscricaoMunicipal FromString(string value) => new(value);

    /// <summary>Cria uma <see cref="InscricaoMunicipal"/> a partir de um número inteiro longo.</summary>
    /// <param name="value">Valor numérico da IM.</param>
    /// <returns>Nova instância de <see cref="InscricaoMunicipal"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static InscricaoMunicipal FromInt64(long value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator InscricaoMunicipal(string value) => FromString(value);

    /// <inheritdoc cref="FromInt64"/>
    public static explicit operator InscricaoMunicipal(long value) => FromInt64(value);
}