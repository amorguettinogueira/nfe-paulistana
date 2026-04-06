using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Código de endereçamento postal para endereços no exterior, com no máximo 11 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpCodigoEndPostal</c>, Linha: 123.
/// </remarks>
[Serializable]
public sealed class CodigoEndPostal : ConstrainedString
{
    private const int MaxLength = 11;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoEndPostal()
    { }

    /// <summary>
    /// Inicializa o Value Object com o Código de endereçamento postal informado.
    /// </summary>
    /// <param name="value">Código de endereçamento postal (máximo 11 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 11 caracteres.
    /// </exception>
    public CodigoEndPostal(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="CodigoEndPostal"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Código de endereçamento postal.</param>
    /// <returns>Nova instância de <see cref="CodigoEndPostal"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoEndPostal FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="CodigoEndPostal"/>.
    /// </summary>
    /// <param name="value">Código de endereçamento postal.</param>
    public static explicit operator CodigoEndPostal(string value) => FromString(value);
}