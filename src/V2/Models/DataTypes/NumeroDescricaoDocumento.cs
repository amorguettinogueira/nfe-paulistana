using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa número e descrição de documentos, fiscais ou não.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpXNomeEvt</c>, Linha: 733.
/// </remarks>
[Serializable]
public sealed class NumeroDescricaoDocumento : ConstrainedString
{
    private const int MaxLength = 255;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NumeroDescricaoDocumento()
    { }

    /// <summary>
    /// Inicializa o Value Object com o número e descrição do documento informado.
    /// </summary>
    /// <param name="value">Número e descrição do documento (máximo 255 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 255 caracteres.
    /// </exception>
    public NumeroDescricaoDocumento(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="NumeroDescricaoDocumento"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Número e descrição do documento.</param>
    /// <returns>Nova instância de <see cref="NumeroDescricaoDocumento"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static NumeroDescricaoDocumento FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="NumeroDescricaoDocumento"/>.
    /// </summary>
    /// <param name="value">Número e descrição do documento.</param>
    public static explicit operator NumeroDescricaoDocumento(string value) => FromString(value);
}