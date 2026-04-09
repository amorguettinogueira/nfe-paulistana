using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Código da lista de Nomenclatura Comum do Mercosul (NCM).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCodigoNCM</c>: <c>[0-9]{8}</c>.
/// </remarks>
[Serializable]
public sealed partial class CodigoNCM : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O código NCM informado \"{0}\" não atende ao formato exigido: exatamente 8 caracteres numéricos.";

    [GeneratedRegex(@"^[0-9]{8}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoNCM() { }

    /// <summary>
    /// Inicializa o Value Object com o código NCM informado.
    /// </summary>
    /// <param name="value">Código NCM (exatamente 8 caracteres numéricos <c>[0-9]{8}</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9]{8}</c>.
    /// </exception>
    public CodigoNCM(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="CodigoNCM"/> a partir de uma string.</summary>
    /// <param name="value">Código NCM.</param>
    /// <returns>Nova instância de <see cref="CodigoNCM"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoNCM FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CodigoNCM(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo CodigoNCM não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="CodigoNCM"/>.
    /// </summary>
    /// <param name="value">Código NCM como string, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="CodigoNCM"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoNCM? ParseIfPresent(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : FromString(value);
}