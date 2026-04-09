using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa a Chave do Documento Fiscal eletrônico do repositório nacional referenciado para os casos de operações já tributadas.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpChaveDFe</c>, Linha: 723.
/// </remarks>
[Serializable]
public sealed class ChaveDocumentoFiscal : ConstrainedString
{
    private const int MaxLength = 50;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ChaveDocumentoFiscal()
    { }

    /// <summary>
    /// Inicializa o Value Object com a Chave do Documento Fiscal eletrônico do repositório nacional referenciado para os casos de operações já tributadas.
    /// </summary>
    /// <param name="value">Chave do Documento Fiscal eletrônico (máximo 50 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 50 caracteres.
    /// </exception>
    public ChaveDocumentoFiscal(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="ChaveDocumentoFiscal"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Chave do Documento Fiscal eletrônico.</param>
    /// <returns>Nova instância de <see cref="ChaveDocumentoFiscal"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static ChaveDocumentoFiscal FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="ChaveDocumentoFiscal"/>.
    /// </summary>
    /// <param name="value">Chave do Documento Fiscal eletrônico.</param>
    public static explicit operator ChaveDocumentoFiscal(string value) => FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="ChaveDocumentoFiscal"/>.
    /// </summary>
    /// <param name="value">Chave do Documento Fiscal eletrônico, possivelmente nula ou vazia.</param>
    /// <returns>Nova instância de <see cref="ChaveDocumentoFiscal"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> exceder 50 caracteres.</exception>
    public static ChaveDocumentoFiscal? ParseIfPresent(string? value) =>
        ParseIfPresent(value, FromString);
}