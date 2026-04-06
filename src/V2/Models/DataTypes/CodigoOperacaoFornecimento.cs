using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object que representa o Código indicador da operação de fornecimento, conforme tabela "código indicador de operação" publicada no ANEXO AnexoVII-IndOp_IBSCBS_V1.00.00.xlsx.
/// </summary>
/// <remarks>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCIndOp</c>: <c>[0-9]{6}</c>.
/// </remarks>
[Serializable]
public sealed partial class CodigoOperacaoFornecimento : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O código NBS informado \"{0}\" não atende ao formato exigido: exatamente 6 caracteres numéricos.";

    [GeneratedRegex(@"^[0-9]{6}$")]
    private static partial Regex ChavePattern();

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoOperacaoFornecimento() { }

    /// <summary>
    /// Inicializa o Value Object com o código da operação de fornecimento informado.
    /// </summary>
    /// <param name="value">Código da operação de fornecimento (exatamente 6 caracteres numéricos <c>[0-9]{6}</c>).</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for nulo, vazio ou não atender ao padrão <c>[0-9]{6}</c>.
    /// </exception>
    public CodigoOperacaoFornecimento(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!ChavePattern().IsMatch(value))
        {
            throw new ArgumentException(FormatoInvalido.Format(value), nameof(value));
        }

        Value = value;
    }

    /// <summary>Cria uma instância de <see cref="CodigoOperacaoFornecimento"/> a partir de uma string.</summary>
    /// <param name="value">Código da operação de fornecimento.</param>
    /// <returns>Nova instância de <see cref="CodigoOperacaoFornecimento"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoOperacaoFornecimento FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CodigoOperacaoFornecimento(string value) => FromString(value);

    /// <inheritdoc/>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo CodigoOperacaoFornecimento não pode ser nulo ou vazio.");
        }

        if (!ChavePattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }
    }
}