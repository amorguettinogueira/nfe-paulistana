using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Código da lista de Nomenclatura Brasileira de Serviços (NBS).
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCodigoNBS</c>: <c>[0-9]{9}</c>.
/// </remarks>
[Serializable]
public sealed partial class CodigoNBS : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O código NBS informado \"{0}\" não atende ao formato exigido: exatamente 9 caracteres numéricos.";

    [GeneratedRegex(@"^[0-9]{9}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoNBS() { }

    /// <summary>
    /// Inicializa o Value Object com o código NBS informado.
    /// </summary>
    /// <param name="value">Código NBS (exatamente 9 caracteres numéricos <c>[0-9]{9}</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9]{9}</c>.
    /// </exception>
    public CodigoNBS(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="CodigoNBS"/> a partir de uma string.</summary>
    /// <param name="value">Código NBS.</param>
    /// <returns>Nova instância de <see cref="CodigoNBS"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoNBS FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CodigoNBS(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo CodigoNBS não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }
}