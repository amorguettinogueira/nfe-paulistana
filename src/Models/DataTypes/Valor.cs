namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object monetário que armazena valores em R$ com até 15 dígitos inteiros
/// e 2 casas decimais, conforme o XSD da NF-e Paulistana (<c>tpValor</c>).
/// </summary>
/// <remarks>
/// <para>
/// Valores inteiros são serializados sem casas decimais (<c>"1000"</c>);
/// valores fracionários sempre com exatamente 2 casas (<c>"1000.50"</c>).
/// </para>
/// <para>
/// <see cref="FromValor"/> e o operador de conversão implícita retornam
/// <c>0</c> (<see cref="decimal.Zero"/>) quando o valor é <c>null</c> ou vazio,
/// pois <see cref="decimal"/> é um tipo de valor não-anulável.
/// </para>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpValor</c>, linha 344.</para>
/// </remarks>
[Serializable]
public sealed class Valor : ConstrainedDecimal
{
    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    public Valor()
    { }

    /// <summary>Inicializa o valor monetário com validação dos limites do XSD.</summary>
    /// <param name="value">Valor em R$. Deve ser não-negativo e não superior a <c>999.999.999.999.999,99</c>.</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou exceder o valor máximo permitido.</exception>
    public Valor(decimal value) : base(value)
    { }

    /// <summary>Cria um <see cref="Valor"/> a partir de um valor decimal.</summary>
    /// <param name="value">Valor em R$.</param>
    /// <returns>Nova instância de <see cref="Valor"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou exceder o valor máximo.</exception>
    public static Valor FromDecimal(decimal value) => new(value);

    /// <summary>
    /// Converte um <see cref="Valor"/> para <see cref="decimal"/>.
    /// </summary>
    /// <param name="value">Instância a converter, ou <c>null</c>.</param>
    /// <returns>
    /// Valor decimal correspondente, ou <see cref="decimal.Zero"/> se <paramref name="value"/>
    /// for <c>null</c> ou tiver valor interno vazio (ex: criado pelo construtor padrão).
    /// </returns>
    public static decimal FromValor(Valor? value) => value == null || string.IsNullOrEmpty(value.Value) ? default : decimal.Parse(value.Value, EnglishCulture);

    /// <inheritdoc cref="FromDecimal"/>
    public static explicit operator Valor(decimal value) => FromDecimal(value);

    /// <inheritdoc cref="FromValor"/>
    public static implicit operator decimal(Valor? value) => FromValor(value);

    /// <summary>Cria um <see cref="Valor"/> a partir de um valor <see cref="double"/>.</summary>
    /// <remarks>O valor é convertido para <see cref="decimal"/> via cast direto; erros de representação em ponto flutuante binário são preservados.</remarks>
    /// <param name="value">Valor em R$.</param>
    /// <returns>Nova instância de <see cref="Valor"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou exceder o valor máximo.</exception>
    /// <exception cref="OverflowException">Se <paramref name="value"/> for <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/> ou <see cref="double.NegativeInfinity"/>.</exception>
    public static Valor FromDouble(double value) => new((decimal)value);

    /// <inheritdoc cref="FromDouble"/>
    public static explicit operator Valor(double value) => FromDouble(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="Valor"/>.
    /// </summary>
    /// <param name="value">Valor como número double, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="Valor"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>999.999.999.999.999,99</c>.</exception>
    public static Valor? ParseIfPresent(double? value) =>
        ParseIfPresent(value, FromDouble);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="Valor"/>.
    /// </summary>
    /// <param name="value">Valor como número decimal, possivelmente nulo.</param>
    /// <returns>Nova instância de <see cref="Valor"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>999.999.999.999.999,99</c>.</exception>
    public static Valor? ParseIfPresent(decimal? value) =>
        ParseIfPresent(value, FromDecimal);
}