using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o complemento do endereço, com no máximo 30 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpComplementoEndereco</c>, Linha: 108.
/// </remarks>
[Serializable]
public sealed class Complemento : ConstrainedString
{
    private const int MaxLength = 30;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Complemento()
    { }

    /// <summary>
    /// Inicializa o Value Object com o complemento do endereço informado.
    /// </summary>
    /// <param name="value">Complemento do endereço (máximo 30 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 30 caracteres.
    /// </exception>
    public Complemento(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="Complemento"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Complemento do endereço.</param>
    /// <returns>Nova instância de <see cref="Complemento"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Complemento FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Complemento"/>.
    /// </summary>
    /// <param name="value">Complemento do endereço.</param>
    public static explicit operator Complemento(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="Complemento"/>.
    /// </summary>
    /// <param name="value">Complemento do endereço, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="Complemento"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 30 caracteres.</exception>
    public static Complemento? ParseIfPresent(string? value) =>
        ParseIfPresent(value, v => new Complemento(v));
}