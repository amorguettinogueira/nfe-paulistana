using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Tipo Nome do evento cultural, artístico, esportivo, com no máximo 255 caracteres.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpXNomeEvt</c>, Linha: 790.
/// </remarks>
[Serializable]
public sealed class NomeEvento : ConstrainedString
{
    private const int MaxLength = 255;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public NomeEvento()
    { }

    /// <summary>
    /// Inicializa o Value Object com o Nome do evento informado.
    /// </summary>
    /// <param name="value">Nome do evento (máximo 255 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 255 caracteres.
    /// </exception>
    public NomeEvento(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="NomeEvento"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Nome do evento.</param>
    /// <returns>Nova instância de <see cref="NomeEvento"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static NomeEvento FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="NomeEvento"/>.
    /// </summary>
    /// <param name="value">Nome do evento.</param>
    public static explicit operator NomeEvento(string value) => FromString(value);
}