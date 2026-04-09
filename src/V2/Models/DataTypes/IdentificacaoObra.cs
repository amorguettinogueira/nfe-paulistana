using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa a Identificação da obra, do Cadastro Nacional de Obras, ou do Cadastro Específico do INSS.
/// 30 caracteres alfanuméricos maiúsculos.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpIdentificacaoObra</c>: <c>[0-9A-Z]{30}</c>.
/// </remarks>
[Serializable]
public sealed partial class IdentificacaoObra : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "A identificação da obra informada \"{0}\" não atende ao formato exigido: exatamente 30 caracteres alfanuméricos maiúsculos ([0-9A-Z]).";

    [GeneratedRegex(@"^[0-9A-Z]{30}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IdentificacaoObra() { }

    /// <summary>
    /// Inicializa o Value Object com a identificação da obra informada.
    /// </summary>
    /// <param name="value">Identificação da obra (exatamente 30 caracteres <c>[0-9A-Z]</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9A-Z]{30}</c>.
    /// </exception>
    public IdentificacaoObra(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="IdentificacaoObra"/> a partir de uma string.</summary>
    /// <param name="value">Identificação da obra.</param>
    /// <returns>Nova instância de <see cref="IdentificacaoObra"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static IdentificacaoObra FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator IdentificacaoObra(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo IdentificacaoObra não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="IdentificacaoObra"/>.
    /// </summary>
    /// <param name="value">Identificação da obra como string, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="IdentificacaoObra"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static IdentificacaoObra? ParseIfPresent(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : FromString(value);
}