using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Models.DataTypes;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Nfe.Paulistana.V2.Models.DataTypes;

/// <summary>
/// Value Object para CNPJ alfanumérico conforme o formato v02 da NF-e Paulistana.
/// Aceita caracteres alfanuméricos nos 12 primeiros dígitos e numéricos nos 2 últimos.
/// </summary>
/// <remarks>
/// <para>
/// Fonte: <c>TiposNFe_v02.xsd</c> — Tipo <c>tpCNPJ</c>: <c>[0-9A-Z]{12}[0-9]{2}</c>.
/// </para>
/// <para>
/// Diferencia-se do CNPJ v01 (puramente numérico com validação módulo 11) por aceitar
/// caracteres alfanuméricos maiúsculos, conforme a nova especificação da Receita Federal.
/// </para>
/// </remarks>
[Serializable]
public sealed partial class Cnpj : XmlSerializableDataType
{
    private const string FormatoInvalido =
        "O CNPJ informado \"{0}\" não atende ao formato alfanumérico exigido: XX.XXX.XXX/XXXX-99.";

    [GeneratedRegex(@"^[0-9A-Z]{12}[0-9]{2}$")]
    private static partial Regex CnpjPattern();

    /// <summary>
    /// Os pesos utilizados no cálculo do módulo 11 para CNPJ.
    /// </summary>
    private static readonly byte[] ValidationWeights = [2, 3, 4, 5, 6, 7, 8, 9];

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Cnpj() { }

    /// <summary>
    /// Inicializa o CNPJ a partir de uma string alfanumérica de 14 posições.
    /// </summary>
    /// <param name="value">CNPJ alfanumérico sem formatação (14 caracteres).</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> não atender ao padrão <c>[0-9A-Z]{12}[0-9]{2}</c>.</exception>
    public Cnpj(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string normalized = RemoveFormatting(value);

        if (!CnpjPattern().IsMatch(normalized))
        {
            throw new ArgumentException(FormatoInvalido.Format(value));
        }

        Span<int> digits = stackalloc int[14];
        ConvertToDigits(normalized, digits);

        if (!ModulusElevenCalculator.Validate(digits, ValidationWeights))
        {
            throw new ArgumentException($"O CNPJ informado \"{value}\" não é válido segundo o algoritmo de módulo 11.");
        }

        Value = normalized;
    }

    /// <summary>Cria uma instância de <see cref="Cnpj"/> a partir de uma string.</summary>
    /// <param name="value">CNPJ alfanumérico sem formatação.</param>
    /// <returns>Nova instância de <see cref="Cnpj"/> validada.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static Cnpj FromString(string value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator Cnpj(string value) => FromString(value);

    /// <summary>
    /// Hook chamado após desserialização XML (via IXmlSerializable.ReadXml).
    /// Valida o valor desserializado do XML.
    /// </summary>
    /// <exception cref="SerializationException">
    /// Se o valor desserializado não atender ao formato ou validação de módulo 11.
    /// </exception>
    protected override void OnXmlDeserialized()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new SerializationException("O valor desserializado do campo CNPJ não pode ser nulo ou vazio.");
        }

        if (!CnpjPattern().IsMatch(Value))
        {
            throw new SerializationException(FormatoInvalido.Format(Value));
        }

        Span<int> digits = stackalloc int[14];
        ConvertToDigits(Value, digits);

        if (!ModulusElevenCalculator.Validate(digits, ValidationWeights))
        {
            throw new SerializationException($"O valor desserializado do campo CNPJ \"{Value}\" não é válido segundo o algoritmo de módulo 11.");
        }
    }

    /// <summary>
    /// Converte cada caractere alfanumérico em seu valor numérico usando offset ASCII.
    /// Dígitos '0'-'9' mapeiam para 0-9. Letras 'A'-'Z' mapeiam para 17-42.
    /// </summary>
    /// <remarks>
    /// O mapeamento por offset ASCII (<c>c - '0'</c>) é equivalente à tabela oficial
    /// da Receita Federal, onde a faixa 10-16 não é utilizada.
    /// </remarks>
    private static void ConvertToDigits(ReadOnlySpan<char> source, Span<int> destination)
    {
        for (int i = 0; i < source.Length; i++)
        {
            destination[i] = source[i] - '0';
        }
    }

    /// <summary>
    /// Remove caracteres de formatação (pontos, barras, hífens) e normaliza para maiúsculas.
    /// Mantém apenas caracteres alfanuméricos.
    /// </summary>
    private static string RemoveFormatting(string value)
    {
        ReadOnlySpan<char> source = value.AsSpan().Trim();
        Span<char> buffer = stackalloc char[source.Length];
        int pos = 0;

        foreach (char c in source)
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                buffer[pos++] = char.ToUpperInvariant(c);
            }
        }

        return new string(buffer[..pos]);
    }
}