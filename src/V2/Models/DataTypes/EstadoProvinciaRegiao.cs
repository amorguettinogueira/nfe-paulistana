using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Estado, província ou região para endereços no exterior, com no máximo 60 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpEstadoProvinciaRegiao</c>, Linha: 242.
/// </remarks>
[Serializable]
public sealed class EstadoProvinciaRegiao : ConstrainedString
{
    private const int MaxLength = 60;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public EstadoProvinciaRegiao()
    { }

    /// <summary>
    /// Inicializa o Value Object com o Estado, província ou região informado.
    /// </summary>
    /// <param name="value">Estado, província ou região (máximo 60 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 60 caracteres.
    /// </exception>
    public EstadoProvinciaRegiao(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="EstadoProvinciaRegiao"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Estado, província ou região.</param>
    /// <returns>Nova instância de <see cref="EstadoProvinciaRegiao"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static EstadoProvinciaRegiao FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="EstadoProvinciaRegiao"/>.
    /// </summary>
    /// <param name="value">Estado, província ou região.</param>
    public static explicit operator EstadoProvinciaRegiao(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="EstadoProvinciaRegiao"/>.
    /// </summary>
    /// <param name="value">Estado, província ou região, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="EstadoProvinciaRegiao"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 60 caracteres.</exception>
    public static EstadoProvinciaRegiao? ParseIfPresent(string? value) =>
        ParseIfPresent(value, v => new EstadoProvinciaRegiao(v));
}