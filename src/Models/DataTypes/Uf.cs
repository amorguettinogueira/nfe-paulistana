using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa a sigla da Unidade Federativa (UF), com exatamente 2 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpUF</c>, Linha: 334.
/// </remarks>
[Serializable]
public sealed class Uf : ConstrainedString
{
    private const string InvalidLength = "A UF deve ter exatamente 2 caracteres.";
    private const int ExactLength = 2;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Uf()
    { }

    /// <summary>
    /// Inicializa o Value Object com a sigla da UF informada.
    /// </summary>
    /// <param name="value">Sigla da UF (exatamente 2 caracteres, ex.: "SP", "RJ").</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou não tiver exatamente 2 caracteres.
    /// </exception>
    public Uf(string value) : base(EnsureValidUf(value))
    { }

    private static string EnsureValidUf(string value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim().Length != ExactLength
            ? throw new ArgumentException(InvalidLength)
            : value;

    /// <inheritdoc />
    protected override int GetMaxLength() => ExactLength;

    /// <summary>
    /// Cria uma instância de <see cref="Uf"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Sigla da UF.</param>
    /// <returns>Nova instância de <see cref="Uf"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Uf FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Uf"/>.
    /// </summary>
    /// <param name="value">Sigla da UF.</param>
    public static explicit operator Uf(string value) => FromString(value);
}