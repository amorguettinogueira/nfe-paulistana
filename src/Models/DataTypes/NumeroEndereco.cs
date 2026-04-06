using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o número do endereço, com no máximo 10 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpNumeroEndereco</c>, Linha: 190.
/// </remarks>
[Serializable]
public sealed class NumeroEndereco : ConstrainedString
{
    private const int MaxLength = 10;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NumeroEndereco()
    { }

    /// <summary>
    /// Inicializa o Value Object com o número do endereço informado.
    /// </summary>
    /// <param name="value">Número do endereço (máximo 10 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 10 caracteres.
    /// </exception>
    public NumeroEndereco(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="NumeroEndereco"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Número do endereço.</param>
    /// <returns>Nova instância de <see cref="NumeroEndereco"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static NumeroEndereco FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="NumeroEndereco"/>.
    /// </summary>
    /// <param name="value">Número do endereço.</param>
    public static explicit operator NumeroEndereco(string value) => FromString(value);
}