using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.V1.Models.DataTypes;

/// <summary>
/// Value Object para Inscrição Municipal (IM), serializada com zero-padding à esquerda
/// para 8 dígitos, conforme exigido pelo XSD da NF-e Paulistana.
/// </summary>
/// <remarks>
/// <para>
/// Exemplo: o valor <c>39616924</c> é armazenado como <c>"39616924"</c>;
/// o valor <c>1</c> é armazenado como <c>"00000001"</c>.
/// </para>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpInscricaoMunicipal</c>, linha 169.</para>
/// </remarks>
[Serializable]
public sealed class InscricaoMunicipal : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em uma inscrição municipal por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "A inscrição municipal deve ter no máximo 8 dígitos (máximo 99.999.999).";
    private const string InvalidMinValue = "A inscrição municipal deve ter pelo menos 1 dígito significativo (mínimo 1).";
    private const int MinValue = 1;
    private const int MaxValue = 99_999_999;
    private const int InscricaoMunicipalFormattingLength = 8;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InscricaoMunicipal()
    { }

    /// <summary>Inicializa a Inscrição Municipal a partir de um número inteiro, aplicando zero-padding para 8 dígitos.</summary>
    /// <param name="value">Valor numérico da IM. Deve estar entre <c>1</c> e <c>99.999.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public InscricaoMunicipal(int value)
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
        : this(int.TryParse(value.RemoveFormatting(), out int result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria uma <see cref="InscricaoMunicipal"/> a partir de uma string.</summary>
    /// <param name="value">IM como string.</param>
    /// <returns>Nova instância de <see cref="InscricaoMunicipal"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static InscricaoMunicipal FromString(string value) => new(value);

    /// <summary>Cria uma <see cref="InscricaoMunicipal"/> a partir de um número inteiro.</summary>
    /// <param name="value">Valor numérico da IM.</param>
    /// <returns>Nova instância de <see cref="InscricaoMunicipal"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static InscricaoMunicipal FromInt32(int value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator InscricaoMunicipal(string value) => FromString(value);

    /// <inheritdoc cref="FromInt32"/>
    public static explicit operator InscricaoMunicipal(int value) => FromInt32(value);
}