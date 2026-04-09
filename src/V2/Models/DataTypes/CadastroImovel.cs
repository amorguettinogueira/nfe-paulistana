using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Cadastro de imóveis. Obrigatório para construção civil.
/// 50 caracteres alfanuméricos maiúsculos.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCadastroImovel</c>: <c>[0-9A-Z]{50}</c>.
/// </remarks>
[Serializable]
public sealed partial class CadastroImovel : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O cadastro de imóvel informado \"{0}\" não atende ao formato exigido: exatamente 8 caracteres alfanuméricos maiúsculos ([0-9A-Z]).";

    [GeneratedRegex(@"^[0-9A-Z]{8}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CadastroImovel() { }

    /// <summary>
    /// Inicializa o Value Object com o cadastro de imóvel informado.
    /// </summary>
    /// <param name="value">Cadastro de imóvel (exatamente 8 caracteres <c>[0-9A-Z]</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9A-Z]{8}</c>.
    /// </exception>
    public CadastroImovel(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="CadastroImovel"/> a partir de uma string.</summary>
    /// <param name="value">Cadastro de imóvel.</param>
    /// <returns>Nova instância de <see cref="CadastroImovel"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CadastroImovel FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CadastroImovel(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo CadastroImovel não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="CadastroImovel"/>.
    /// </summary>
    /// <param name="value">Cadastro de imóvel como string, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="CadastroImovel"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CadastroImovel? ParseIfPresent(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : FromString(value);
}