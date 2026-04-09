using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o tipo de tributação da NF-e, armazenado como um único caractere.
/// </summary>
/// <remarks>
/// <para>
/// A modelagem como caractere (em vez de enum) é intencional: evita que alterações
/// nos valores permitidos pelo XSD exijam uma nova versão da biblioteca.
/// </para>
/// <para>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpTributacaoNFe</c>, Linha: 324.
/// </para>
/// </remarks>
[Serializable]
public sealed class TributacaoNfe : XmlSerializableDataType
{
    private const string InvalidChar = "O tipo de tributação deve ser representado por uma única letra (ex.: 'T', 'F', 'A').";

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TributacaoNfe()
    { }

    /// <summary>
    /// Inicializa o Value Object com o caractere de tributação informado.
    /// </summary>
    /// <param name="value">Caractere que representa o tipo de tributação (deve ser uma letra).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> não for uma letra.
    /// </exception>
    public TributacaoNfe(char value)
    {
        if (!char.IsLetter(value))
        {
            throw new ArgumentException(InvalidChar);
        }

        Value = value.ToString();
    }

    /// <summary>
    /// Cria uma instância de <see cref="TributacaoNfe"/> a partir de um caractere.
    /// </summary>
    /// <param name="value">Caractere do tipo de tributação.</param>
    /// <returns>Nova instância de <see cref="TributacaoNfe"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> não for uma letra.</exception>
    public static TributacaoNfe FromChar(char value) => new(value);

    /// <summary>
    /// Converte explicitamente um caractere em <see cref="TributacaoNfe"/>.
    /// </summary>
    /// <param name="value">Caractere do tipo de tributação.</param>
    public static explicit operator TributacaoNfe(char value) => FromChar(value);

    /// <summary>
    /// Cria uma instância de <see cref="TributacaoNfe"/> a partir de uma string.
    /// </summary>
    /// <param name="value">String com o tipo de tributação.</param>
    /// <returns>Nova instância de <see cref="TributacaoNfe"/>.</returns>
    /// <exception cref="ArgumentNullException">Se <paramref name="value"/> for nulo.</exception>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for vazio.</exception>
    public static TributacaoNfe FromString(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        return value.Length == 0
            ? throw new ArgumentException("A letra do tipo de tributação não pode ser vazia.", nameof(value))
            : new(value[0]);
    }

    /// <summary>
    /// Converte explicitamente uma string em <see cref="TributacaoNfe"/>.
    /// </summary>
    /// <param name="value">String com o tipo de tributação.</param>
    public static explicit operator TributacaoNfe(string value) => FromString(value);
}