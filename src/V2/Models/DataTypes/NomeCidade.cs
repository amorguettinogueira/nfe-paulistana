using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Nome da Cidade para endereços no exterior, com no máximo 60 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpNomeCidade</c>, Linha: 338.
/// </remarks>
[Serializable]
public sealed class NomeCidade : ConstrainedString
{
    private const int MaxLength = 60;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NomeCidade()
    { }

    /// <summary>
    /// Inicializa o Value Object com o Nome da Cidade informado.
    /// </summary>
    /// <param name="value">Nome da Cidade (máximo 60 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 60 caracteres.
    /// </exception>
    public NomeCidade(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="NomeCidade"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Nome da Cidade.</param>
    /// <returns>Nova instância de <see cref="NomeCidade"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static NomeCidade FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="NomeCidade"/>.
    /// </summary>
    /// <param name="value">Nome da Cidade.</param>
    public static explicit operator NomeCidade(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="NomeCidade"/>.
    /// </summary>
    /// <param name="value">Nome da Cidade, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="NomeCidade"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 60 caracteres.</exception>
    public static NomeCidade? ParseIfPresent(string? value) =>
        ParseIfPresent(value, v => new NomeCidade(v));
}