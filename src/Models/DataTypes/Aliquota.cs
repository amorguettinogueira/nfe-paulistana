namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object para alíquota de ISS, restrito ao intervalo <c>[0, 9,9999]</c>
/// com até 1 dígito inteiro e 4 casas decimais, conforme o XSD da NF-e Paulistana
/// (<c>tpAliquota</c>).
/// </summary>
/// <remarks>
/// <para>
/// A alíquota é armazenada como fração decimal: <c>5%</c> = <c>0.05</c>,
/// <c>10%</c> = <c>0.1</c>. O valor máximo aceito é <c>9.9999</c>.
/// </para>
/// <para>Fonte: <c>TiposNFe_v01.xsd</c> — Tipo <c>tpAliquota</c>, linha 7.</para>
/// </remarks>
[Serializable]
public sealed class Aliquota : ConstrainedDecimal
{
    private const int IntegralMaxLength = 1;
    private const int FractionalMaxLength = 4;

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    public Aliquota()
    { }

    /// <summary>Inicializa a alíquota com validação dos limites do XSD.</summary>
    /// <param name="value">Alíquota como fração decimal (ex: <c>0.05</c> para 5%).</param>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>9.9999</c>.</exception>
    public Aliquota(decimal value) : base(value)
    { }

    /// <inheritdoc/>
    protected override (int integral, int fractional) GetMaxLimit() =>
        (IntegralMaxLength, FractionalMaxLength);

    /// <summary>Cria um <see cref="Aliquota"/> a partir de um valor decimal.</summary>
    /// <param name="value">Alíquota como fração decimal.</param>
    /// <returns>Nova instância de <see cref="Aliquota"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>9.9999</c>.</exception>
    public static Aliquota FromDecimal(decimal value) => new(value);

    /// <inheritdoc cref="FromDecimal"/>
    public static explicit operator Aliquota(decimal value) => FromDecimal(value);

    /// <summary>Cria um <see cref="Aliquota"/> a partir de um valor <see cref="double"/>.</summary>
    /// <remarks>O valor é convertido para <see cref="decimal"/> via cast direto; erros de representação em ponto flutuante binário são preservados.</remarks>
    /// <param name="value">Alíquota como fração decimal (ex: <c>0.05</c> para 5%).</param>
    /// <returns>Nova instância de <see cref="Aliquota"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for negativo ou superior a <c>9.9999</c>.</exception>
    /// <exception cref="OverflowException">Se <paramref name="value"/> for <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/> ou <see cref="double.NegativeInfinity"/>.</exception>
    public static Aliquota FromDouble(double value) => new((decimal)value);

    /// <inheritdoc cref="FromDouble"/>
    public static explicit operator Aliquota(double value) => FromDouble(value);
}