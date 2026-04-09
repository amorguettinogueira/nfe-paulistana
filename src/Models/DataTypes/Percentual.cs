namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para percentual de carga tributária, restrito ao intervalo <c>[0, 100]</c>
/// com até 3 dígitos inteiros e 4 casas decimais, conforme o XSD da NF-e Paulistana
/// (<c>tpPercentualCargaTributaria</c>).
/// </summary>
/// <remarks>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpPercentualCargaTributaria</c>, linha 363.</para>
/// </remarks>
[Serializable]
public sealed class Percentual : ConstrainedDecimal
{
    private const string InvalidNumber = "O percentual não deve ser maior do que 100%.";
    private const int MaxValue = 100;
    private const int IntegralMaxLength = 3;
    private const int FractionalMaxLength = 4;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    public Percentual()
    { }

    /// <summary>Inicializa o percentual com validação dos limites do XSD.</summary>
    /// <param name="value">Percentual como número (ex: <c>15.5</c> para 15,5%). Deve estar entre <c>0</c> e <c>100</c>.</param>
    /// <exception cref="ArgumentException">
    /// Se <paramref name="value"/> for negativo ou superior a <c>100</c>.
    /// </exception>
    public Percentual(decimal value) : base(ValidateRange(value))
    { }

    /// <summary>Valida o limite de 100% antes de chamar o construtor base, evitando que <c>Value</c> seja definido com dado inválido.</summary>
    private static decimal ValidateRange(decimal value) =>
        value <= MaxValue ? value
            : throw new ArgumentException(InvalidNumber);

    /// <inheritdoc/>
    protected override (int integral, int fractional) GetMaxLimit() =>
        (IntegralMaxLength, FractionalMaxLength);

    /// <summary>Cria um <see cref="Percentual"/> a partir de um valor decimal.</summary>
    /// <param name="value">Percentual entre <c>0</c> e <c>100</c>.</param>
    /// <returns>Nova instância de <see cref="Percentual"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>100</c>.</exception>
    public static Percentual FromDecimal(decimal value) => new(value);

    /// <inheritdoc cref="FromDecimal"/>
    public static explicit operator Percentual(decimal value) => FromDecimal(value);

    /// <summary>Cria um <see cref="Percentual"/> a partir de um valor <see cref="double"/>.</summary>
    /// <remarks>O valor é convertido para <see cref="decimal"/> via cast direto; erros de representação em ponto flutuante binário são preservados.</remarks>
    /// <param name="value">Percentual como número (ex: <c>15.5</c> para 15,5%).</param>
    /// <returns>Nova instância de <see cref="Percentual"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>100</c>.</exception>
    /// <exception cref="OverflowException">Se <paramref name="value"/> for <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/> ou <see cref="double.NegativeInfinity"/>.</exception>
    public static Percentual FromDouble(double value) => new((decimal)value);

    /// <inheritdoc cref="FromDouble"/>
    public static explicit operator Percentual(double value) => FromDouble(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="Percentual"/>.
    /// </summary>
    /// <param name="value">Percentual como número double, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="Percentual"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>100</c>.</exception>
    public static Percentual? ParseIfPresent(double? value) =>
        ParseIfPresent(value, FromDouble);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="Percentual"/>.
    /// </summary>
    /// <param name="value">Percentual como número decimal, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="Percentual"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>100</c>.</exception>
    public static Percentual? ParseIfPresent(decimal? value) =>
        ParseIfPresent(value, FromDecimal);
}