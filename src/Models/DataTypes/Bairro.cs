using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o nome do bairro do endereço, com no máximo 30 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpBairro</c>, Linha: 48.
/// </remarks>
[Serializable]
public sealed class Bairro : ConstrainedString
{
    private const int MaxLength = 30;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Bairro()
    { }

    /// <summary>
    /// Inicializa o Value Object com o nome do bairro informado.
    /// </summary>
    /// <param name="value">Nome do bairro (máximo 30 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 30 caracteres.
    /// </exception>
    public Bairro(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="Bairro"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Nome do bairro.</param>
    /// <returns>Nova instância de <see cref="Bairro"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Bairro FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Bairro"/>.
    /// </summary>
    /// <param name="value">Nome do bairro.</param>
    public static explicit operator Bairro(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="Bairro"/>.
    /// </summary>
    /// <param name="value">Nome do bairro, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="Bairro"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 30 caracteres.</exception>
    public static Bairro? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}