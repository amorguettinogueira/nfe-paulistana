using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa a fonte da carga tributária, com no máximo 10 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpFonteCargaTributaria</c>, Linha: 373.
/// </remarks>
[Serializable]
public sealed class FonteCargaTributaria : ConstrainedString
{
    private const int MaxLength = 10;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public FonteCargaTributaria()
    { }

    /// <summary>
    /// Inicializa o Value Object com a fonte da carga tributária informada.
    /// </summary>
    /// <param name="value">Fonte da carga tributária (máximo 10 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 10 caracteres.
    /// </exception>
    public FonteCargaTributaria(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="FonteCargaTributaria"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Fonte da carga tributária.</param>
    /// <returns>Nova instância de <see cref="FonteCargaTributaria"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static FonteCargaTributaria FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="FonteCargaTributaria"/>.
    /// </summary>
    /// <param name="value">Fonte da carga tributária.</param>
    public static explicit operator FonteCargaTributaria(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="FonteCargaTributaria"/>.
    /// </summary>
    /// <param name="value">Fonte da carga tributária, possivelmente nula ou vazia.</param>
    /// <returns>Nova instância de <see cref="FonteCargaTributaria"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 10 caracteres.</exception>
    public static FonteCargaTributaria? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}