using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o NIF (Número de Identificação Fiscal) - fornecido por um órgão de administração tributária no exterior, com no máximo 40 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpNIF</c>, Linha: 328.
/// </remarks>
[Serializable]
public sealed class Nif : ConstrainedString
{
    private const int MaxLength = 40;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Nif()
    { }

    /// <summary>
    /// Inicializa o Value Object com o Nome da Cidade informado.
    /// </summary>
    /// <param name="value">Nome da Cidade (máximo 60 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 60 caracteres.
    /// </exception>
    public Nif(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="Nif"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Nome da Cidade.</param>
    /// <returns>Nova instância de <see cref="Nif"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Nif FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Nif"/>.
    /// </summary>
    /// <param name="value">Nome da Cidade.</param>
    public static explicit operator Nif(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="Nif"/>.
    /// </summary>
    /// <param name="value">NIF, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="Nif"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 40 caracteres.</exception>
    public static Nif? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}