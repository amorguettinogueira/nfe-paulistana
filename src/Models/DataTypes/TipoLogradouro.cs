using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o tipo do logradouro (ex.: Rua, Av.), com no máximo 3 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpTipoLogradouro</c>, Linha: 292.
/// </remarks>
[Serializable]
public sealed class TipoLogradouro : ConstrainedString
{
    private const int MaxLength = 3;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TipoLogradouro()
    { }

    /// <summary>
    /// Inicializa o Value Object com o tipo de logradouro informado.
    /// </summary>
    /// <param name="value">Tipo de logradouro (máximo 3 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 3 caracteres.
    /// </exception>
    public TipoLogradouro(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="TipoLogradouro"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Tipo de logradouro.</param>
    /// <returns>Nova instância de <see cref="TipoLogradouro"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static TipoLogradouro FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="TipoLogradouro"/>.
    /// </summary>
    /// <param name="value">Tipo de logradouro.</param>
    public static explicit operator TipoLogradouro(string value) => FromString(value);
}