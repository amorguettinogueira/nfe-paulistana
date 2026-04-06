using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Tipo do código de classificação Tributária do IBS e da CBS.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpClassificacaoTributaria</c>: <c>[0-9]{6}</c>.
/// </remarks>
[Serializable]
public sealed partial class ClassificacaoTributaria : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O código de classificação tributária informado \"{0}\" não atende ao formato exigido: exatamente 6 caracteres numéricos.";

    [GeneratedRegex(@"^[0-9]{6}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ClassificacaoTributaria() { }

    /// <summary>
    /// Inicializa o Value Object com o código de classificação tributária informado.
    /// </summary>
    /// <param name="value">Código de classificação tributária (exatamente 6 caracteres numéricos <c>[0-9]{6}</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9]{6}</c>.
    /// </exception>
    public ClassificacaoTributaria(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="ClassificacaoTributaria"/> a partir de uma string.</summary>
    /// <param name="value">Código de classificação tributária.</param>
    /// <returns>Nova instância de <see cref="ClassificacaoTributaria"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static ClassificacaoTributaria FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator ClassificacaoTributaria(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo ClassificacaoTributaria não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }
}