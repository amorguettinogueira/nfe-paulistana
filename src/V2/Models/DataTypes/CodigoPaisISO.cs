using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Código do País segundo tabela ISO.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCodigoPaisISO</c>: <c>[A-Z]{2}</c>.
/// </remarks>
[Serializable]
public sealed partial class CodigoPaisISO : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O código do país informado \"{0}\" não atende ao formato exigido: exatamente 2 caracteres alfabéticos maiúsculos ([A-Z]).";

    [GeneratedRegex(@"^[A-Z]{2}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoPaisISO() { }

    /// <summary>
    /// Inicializa o Value Object com o código do país informado.
    /// </summary>
    /// <param name="value">Código do país (exatamente 2 caracteres alfabéticos maiúsculos <c>[A-Z]</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[A-Z]{2}</c>.
    /// </exception>
    public CodigoPaisISO(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="CodigoPaisISO"/> a partir de uma string.</summary>
    /// <param name="value">Código do país.</param>
    /// <returns>Nova instância de <see cref="CodigoPaisISO"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoPaisISO FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CodigoPaisISO(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo CodigoPaisISO não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }
}