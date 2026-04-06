using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o nome do logradouro do endereço, com no máximo 50 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpLogradouro</c>, Linha: 172.
/// </remarks>
[Serializable]
public sealed class Logradouro : ConstrainedString
{
    private const int MaxLength = 50;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Logradouro()
    { }

    /// <summary>
    /// Inicializa o Value Object com o nome do logradouro informado.
    /// </summary>
    /// <param name="value">Nome do logradouro (máximo 50 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 50 caracteres.
    /// </exception>
    public Logradouro(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="Logradouro"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Nome do logradouro.</param>
    /// <returns>Nova instância de <see cref="Logradouro"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Logradouro FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Logradouro"/>.
    /// </summary>
    /// <param name="value">Nome do logradouro.</param>
    public static explicit operator Logradouro(string value) => FromString(value);
}