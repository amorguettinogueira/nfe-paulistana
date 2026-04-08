using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa a série do RPS, com no máximo 5 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpSerieRPS</c>, Linha: 245.
/// </remarks>
[Serializable]
public sealed class SerieRps : ConstrainedString
{
    private const int MaxLength = 5;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SerieRps()
    { }

    /// <summary>
    /// Inicializa o Value Object com a série do RPS informada.
    /// </summary>
    /// <param name="value">Série do RPS (máximo 5 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 5 caracteres.
    /// </exception>
    public SerieRps(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="SerieRps"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Série do RPS.</param>
    /// <returns>Nova instância de <see cref="SerieRps"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static SerieRps FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="SerieRps"/>.
    /// </summary>
    /// <param name="value">Série do RPS.</param>
    public static explicit operator SerieRps(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="SerieRps"/>.
    /// </summary>
    /// <param name="value">Série do RPS, possivelmente nula ou vazia.</param>
    /// <returns>Nova instância de <see cref="SerieRps"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 5 caracteres.</exception>
    public static SerieRps? ParseIfPresent(string? value) =>
        ParseIfPresent(value, v => new SerieRps(v));
}