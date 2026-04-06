using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa a chave da Nota Nacional, composta por exatamente
/// 50 caracteres alfanuméricos maiúsculos.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpChaveNotaNacional</c>: <c>[0-9A-Z]{50}</c>.
/// </remarks>
[Serializable]
public sealed partial class ChaveNotaNacional : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "A chave da Nota Nacional informada \"{0}\" não atende ao formato exigido: exatamente 50 caracteres alfanuméricos maiúsculos ([0-9A-Z]).";

    [GeneratedRegex(@"^[0-9A-Z]{50}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ChaveNotaNacional() { }

    /// <summary>
    /// Inicializa o Value Object com a chave da Nota Nacional informada.
    /// </summary>
    /// <param name="value">Chave da Nota Nacional (exatamente 50 caracteres <c>[0-9A-Z]</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9A-Z]{50}</c>.
    /// </exception>
    public ChaveNotaNacional(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="ChaveNotaNacional"/> a partir de uma string.</summary>
    /// <param name="value">Chave da Nota Nacional.</param>
    /// <returns>Nova instância de <see cref="ChaveNotaNacional"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static ChaveNotaNacional FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator ChaveNotaNacional(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo ChaveNotaNacional não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }
}
