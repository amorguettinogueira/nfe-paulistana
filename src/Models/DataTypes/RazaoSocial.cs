using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa a razão social de uma pessoa jurídica ou o nome de uma pessoa física,
/// com no máximo 75 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpRazaoSocial</c>, Linha: 235.
/// </remarks>
[Serializable]
public sealed class RazaoSocial : ConstrainedString
{
    private const int MaxLength = 75;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public RazaoSocial()
    { }

    /// <summary>
    /// Inicializa o Value Object com a razão social informada.
    /// </summary>
    /// <param name="value">Razão social ou nome (máximo 75 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 75 caracteres.
    /// </exception>
    public RazaoSocial(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="RazaoSocial"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Razão social ou nome.</param>
    /// <returns>Nova instância de <see cref="RazaoSocial"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static RazaoSocial FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="RazaoSocial"/>.
    /// </summary>
    /// <param name="value">Razão social ou nome.</param>
    public static explicit operator RazaoSocial(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="RazaoSocial"/>.
    /// </summary>
    /// <param name="value">Razão social ou nome, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="RazaoSocial"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 75 caracteres.</exception>
    public static RazaoSocial? ParseIfPresent(string? value) =>
        ParseIfPresent(value, v => new RazaoSocial(v));
}