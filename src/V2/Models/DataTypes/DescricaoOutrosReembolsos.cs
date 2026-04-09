using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa a Descrição do reembolso ou ressarcimento quando a opção é 99 - Outros reembolsos ou ressarcimentos recebidos por valores pagos relativos a operações por conta e ordem de terceiro, com no máximo 150 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpXTpReeRepRes</c>, Linha: 780.
/// </remarks>
[Serializable]
public sealed class DescricaoOutrosReembolsos : ConstrainedString
{
    private const int MaxLength = 150;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DescricaoOutrosReembolsos()
    { }

    /// <summary>
    /// Inicializa o Value Object com a descrição informada.
    /// </summary>
    /// <param name="value">Descrição do reembolso ou ressarcimento (máximo 150 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 150 caracteres.
    /// </exception>
    public DescricaoOutrosReembolsos(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="DescricaoOutrosReembolsos"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Descrição do reembolso ou ressarcimento.</param>
    /// <returns>Nova instância de <see cref="DescricaoOutrosReembolsos"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static DescricaoOutrosReembolsos FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="DescricaoOutrosReembolsos"/>.
    /// </summary>
    /// <param name="value">Descrição do reembolso ou ressarcimento.</param>
    public static explicit operator DescricaoOutrosReembolsos(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="DescricaoOutrosReembolsos"/>.
    /// </summary>
    /// <param name="value">Descrição do reembolso ou ressarcimento, possivelmente nula ou vazia.</param>
    /// <returns>Nova instância de <see cref="DescricaoOutrosReembolsos"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 150 caracteres.</exception>
    public static DescricaoOutrosReembolsos? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}