using System.ComponentModel;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa o código de verificação, com exatamente 8 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v01.xsd — Tipo: <c>tpCodigoVerificacao</c>, Linha: 48.
/// </remarks>
[Serializable]
public sealed class CodigoVerificacao : ConstrainedString
{
    private const int MaxLength = 8;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoVerificacao()
    { }

    /// <summary>
    /// Valida o valor do código de verificação, garantindo que tenha exatamente 8 caracteres.
    /// </summary>
    /// <param name="value">Código de verificação a ser validado.</param>
    /// <returns>O valor do código de verificação se for válido.</returns>
    /// <exception cref="ArgumentNullException">Se o valor for nulo.</exception>
    /// <exception cref="ArgumentException">Se o valor tiver menos de 8 caracteres.</exception>
    private static string GetValueOrThrow(string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "O código de verificação não pode ser nulo.");
        }

        if (value.Length < MaxLength)
        {
            throw new ArgumentException($"O código de verificação deve ter exatamente {MaxLength} caracteres.", nameof(value));
        }

        return value;
    }

    /// <summary>
    /// Inicializa o Value Object com o nome do bairro informado.
    /// </summary>
    /// <param name="value">Código de verificação (exatamente 8 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou não tiver exatamente 8 caracteres.
    /// </exception>
    public CodigoVerificacao(string value) : base(GetValueOrThrow(value))
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="CodigoVerificacao"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Código de verificação.</param>
    /// <returns>Nova instância de <see cref="CodigoVerificacao"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoVerificacao FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="CodigoVerificacao"/>.
    /// </summary>
    /// <param name="value">Código de verificação.</param>
    public static explicit operator CodigoVerificacao(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="CodigoVerificacao"/>.
    /// </summary>
    /// <param name="value">Código de verificação, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="CodigoVerificacao"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> não tiver exatamente 8 caracteres.</exception>
    public static CodigoVerificacao? ParseIfPresent(string? value) =>
        ParseIfPresent(value, v => new CodigoVerificacao(v));
}