using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa a Inscrição Imobiliária fiscal (código fornecido pela prefeitura para identificação da obra ou para fins de recolhimento do IPTU). Exemplos: SQL ou INCRA.
/// </summary>
/// <remarks>
/// Fonte: TiposNFe_v02.xsd — Tipo: <c>tpInscImobFisc</c>, Linha: 672.
/// </remarks>
[Serializable]
public sealed class InscricaoImobiliaria : ConstrainedString
{
    private const int MaxLength = 30;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InscricaoImobiliaria()
    { }

    /// <summary>
    /// Inicializa o Value Object com a Inscrição Imobiliária fornecida.
    /// </summary>
    /// <param name="value">Inscrição Imobiliária (máximo 30 caracteres).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for vazio ou exceder 30 caracteres.
    /// </exception>
    public InscricaoImobiliaria(string value) : base(value)
    { }

    /// <inheritdoc />
    protected override int GetMaxLength() => MaxLength;

    /// <summary>
    /// Cria uma instância de <see cref="InscricaoImobiliaria"/> a partir de uma string.
    /// </summary>
    /// <param name="value">Inscrição Imobiliária.</param>
    /// <returns>Nova instância de <see cref="InscricaoImobiliaria"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static InscricaoImobiliaria FromString(string value) => new(value);

    /// <summary>
    /// Converte explicitamente uma string em <see cref="InscricaoImobiliaria"/>.
    /// </summary>
    /// <param name="value">Inscrição Imobiliária.</param>
    public static explicit operator InscricaoImobiliaria(string value) => FromString(value);
}