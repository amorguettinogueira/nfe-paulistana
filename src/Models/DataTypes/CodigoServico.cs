using Nfe.Paulistana.Extensions;
using System.ComponentModel;
using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para o código do serviço prestado conforme tabela da Prefeitura de São Paulo,
/// com 4 a 5 dígitos.
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpCodigoServico</c>, linha 82.</para>
/// </remarks>
[Serializable]
public sealed class CodigoServico : XmlSerializableDataType
{
    private const string InvalidNumber = "Não é possível converter \"{0}\" em um código de serviço por conter caracteres não numéricos.";
    private const string InvalidMaxValue = "O código de serviço deve ter no máximo 5 dígitos (máximo 99.999).";
    private const string InvalidMinValue = "O código de serviço deve ter pelo menos 4 dígitos significativos (mínimo 01.000).";
    private const int MinValue = 1_000;
    private const int MaxValue = 99_999;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CodigoServico()
    { }

    /// <summary>Inicializa o código de serviço a partir de um número inteiro.</summary>
    /// <param name="value">Código numérico. Deve estar entre <c>1.000</c> e <c>99.999</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public CodigoServico(int value)
    {
        if (value < MinValue)
        {
            throw new ArgumentException(InvalidMinValue);
        }
        else if (value > MaxValue)
        {
            throw new ArgumentException(InvalidMaxValue);
        }

        Value = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Inicializa o código de serviço a partir de uma string, removendo formatação antes de parsear.</summary>
    /// <param name="value">Código do serviço como string.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> contiver caracteres não numéricos ou estiver fora do intervalo válido.</exception>
    public CodigoServico(string value)
        : this(int.TryParse(value, out int result)
            ? result
            : throw new ArgumentException(InvalidNumber.Format(value)))
    { }

    /// <summary>Cria um <see cref="CodigoServico"/> a partir de uma string.</summary>
    /// <param name="value">Código do serviço como string.</param>
    /// <returns>Nova instância de <see cref="CodigoServico"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoServico FromString(string value) => new(value);

    /// <summary>Cria um <see cref="CodigoServico"/> a partir de um número inteiro.</summary>
    /// <param name="value">Código numérico do serviço.</param>
    /// <returns>Nova instância de <see cref="CodigoServico"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static CodigoServico FromInt32(int value) => new(value);

    /// <inheritdoc cref="FromString"/>
    public static explicit operator CodigoServico(string value) => FromString(value);

    /// <inheritdoc cref="FromInt32"/>
    public static explicit operator CodigoServico(int value) => FromInt32(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo, vazio ou apenas espaços;
    /// caso contrário, cria uma instância de <see cref="CodigoServico"/>.
    /// </summary>
    /// <param name="value">Código do serviço como string, possivelmente nulo ou vazio.</param>
    /// <returns>Nova instância de <see cref="CodigoServico"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static CodigoServico? ParseIfPresent(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : FromString(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="CodigoServico"/>.
    /// </summary>
    /// <param name="value">Código do serviço como número inteiro, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="CodigoServico"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static CodigoServico? ParseIfPresent(int? value) =>
        value == null ? null : FromInt32(value.Value);
}