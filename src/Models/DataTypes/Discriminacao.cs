using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa a descrição do serviço prestado no RPS, com no máximo 2.000 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpDiscriminacao</c>, Linha: 136.
/// </remarks>
[Serializable]
public sealed class Discriminacao : ConstrainedString
{
    private const int MaxLength = 2000;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Discriminacao()
    { }

    /// <summary>
    /// Substitui quebras de linha por pipes ('|') para garantir conformidade com o XSD, que não permite quebras de linha.
    /// </summary>
    /// <param name="text">Texto a ser convertido.</param>
    /// <returns>Texto convertido com pipes no lugar de quebras de linha.</returns>
    private static string ConvertCrLfToPipe(string text) =>
        string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : text.Replace("\r\n", "|", StringComparison.Ordinal).Replace("\n", "|", StringComparison.Ordinal).Replace("\r", "|", StringComparison.Ordinal);

    /// <summary>
    /// Inicializa o Value Object com a descrição do serviço informada.
    /// </summary>
    /// <remarks>
    /// Substitui quebras de linha por pipes ('|') para garantir conformidade com o XSD, que não permite quebras de linha.
    /// </remarks>
    /// <param name="value">Descrição do serviço (máximo 2.000 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 2.000 caracteres.
    /// </exception>
    public Discriminacao(string value) : base(ConvertCrLfToPipe(value))
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="Discriminacao"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Descrição do serviço.</param>
    /// <returns>Nova instância de <see cref="Discriminacao"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Discriminacao FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="Discriminacao"/>.
    /// </summary>
    /// <param name="value">Descrição do serviço.</param>
    public static explicit operator Discriminacao(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="Discriminacao"/>.
    /// </summary>
    /// <param name="value">Descrição do serviço, possivelmente nula ou vazia.</param>
    /// <returns>Nova instância de <see cref="Discriminacao"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 2.000 caracteres.</exception>
    public static Discriminacao? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}