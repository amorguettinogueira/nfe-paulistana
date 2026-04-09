using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa a Descrição da DF -e a que se refere a chaveDfe que seja um dos documentos do Repositório Nacional. Deve ser preenchido apenas quando tipoChaveDFe = 9 (Outro).
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpXTipoChaveDFe</c>, Linha: 713.
/// </remarks>
[Serializable]
public sealed class TipoChaveDfe : ConstrainedString
{
    private const int MaxLength = 255;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TipoChaveDfe()
    { }

    /// <summary>
    /// Inicializa o Value Object com a Descrição da DF-e informada.
    /// </summary>
    /// <param name="value">Descrição da DF-e (máximo 255 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 255 caracteres.
    /// </exception>
    public TipoChaveDfe(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="TipoChaveDfe"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Descrição da DF-e.</param>
    /// <returns>Nova instância de <see cref="TipoChaveDfe"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static TipoChaveDfe FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="TipoChaveDfe"/>.
    /// </summary>
    /// <param name="value">Descrição da DF-e.</param>
    public static explicit operator TipoChaveDfe(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="TipoChaveDfe"/>.
    /// </summary>
    /// <param name="value">Descrição da DF-e, possivelmente nula ou vazia.</param>
    /// <returns>Nova instância de <see cref="TipoChaveDfe"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 255 caracteres.</exception>
    public static TipoChaveDfe? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}