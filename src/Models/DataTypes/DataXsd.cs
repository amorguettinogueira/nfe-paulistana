using System.Globalization;

namespace Nfe.Paulistana.Models.DataTypes;

/// <summary>
/// Value Object que representa uma data no formato <c>yyyy-MM-dd</c> exigido pelo
/// XSD da NF-e Paulistana.
/// </summary>
/// <remarks>
/// <para>
/// A componente de horário do <see cref="DateTime"/> é descartada na construção —
/// apenas a data é armazenada. Fonte: Manual NF-e Web Service v2.8.1, seção 3.4.4.
/// </para>
/// <para>
/// Os métodos <see cref="ToDateTime"/> e <see cref="ToDateOnly"/>, assim como os
/// operadores de conversão implícita, retornam <see cref="DateTime.MinValue"/> e
/// <see cref="DateOnly.MinValue"/> respectivamente quando o valor é <c>null</c> ou
/// vazio, pois são tipos de valor não-anuláveis.
/// </para>
/// </remarks>
[Serializable]
public sealed class DataXsd : XmlSerializableDataType
{
    /// <summary>Formato de data ISO 8601 exigido pelo XSD da NF-e Paulistana.</summary>
    private const string DefaultDateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Construtor sem parâmetros exclusivo para desserialização XML via reflection.
    /// Não deve ser usado pelo código normal.
    /// </summary>
    public DataXsd() : base()
    { }

    /// <summary>
    /// Cria um <see cref="DataXsd"/> a partir de um <see cref="DateTime"/>,
    /// armazenando apenas a data no formato <c>yyyy-MM-dd</c>.
    /// </summary>
    /// <param name="value">Data de referência. O componente de horário é descartado.</param>
    public DataXsd(DateTime value) =>
        Value = value.ToString(DefaultDateFormat, CultureInfo.InvariantCulture);

    /// <summary>
    /// Cria um <see cref="DataXsd"/> a partir de um <see cref="DateOnly"/>,
    /// armazenando apenas a data no formato <c>yyyy-MM-dd</c>.
    /// </summary>
    /// <param name="value">Data de referência.</param>
    public DataXsd(DateOnly value) =>
        Value = value.ToString(DefaultDateFormat, CultureInfo.InvariantCulture);

    /// <summary>
    /// Cria um <see cref="DataXsd"/> a partir de um <see cref="DateTime"/>.
    /// </summary>
    /// <param name="value">Data de referência. O componente de horário é descartado.</param>
    /// <returns>Nova instância de <see cref="DataXsd"/>.</returns>
    public static DataXsd FromDateTime(DateTime value) => new(value);

    /// <summary>
    /// Cria um <see cref="DataXsd"/> a partir de um <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="value">Data de referência.</param>
    /// <returns>Nova instância de <see cref="DataXsd"/>.</returns>
    public static DataXsd FromDateOnly(DateOnly value) => new(value);

    /// <summary>
    /// Converte um <see cref="DataXsd"/> para <see cref="DateTime"/>.
    /// </summary>
    /// <param name="value">Instância a converter, ou <c>null</c>.</param>
    /// <returns>
    /// <see cref="DateTime"/> correspondente à data armazenada, com horário em meia-noite.
    /// Retorna <see cref="DateTime.MinValue"/> se <paramref name="value"/> for <c>null</c>
    /// ou se o valor interno estiver vazio (ex: objeto criado pelo construtor padrão).
    /// </returns>
    public static DateTime ToDateTime(DataXsd? value) => value == null || string.IsNullOrEmpty(value.Value) ? default
        : DateTime.ParseExact(value.Value, DefaultDateFormat, CultureInfo.InvariantCulture);

    /// <summary>
    /// Converte um <see cref="DataXsd"/> para <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="value">Instância a converter, ou <c>null</c>.</param>
    /// <returns>
    /// <see cref="DateOnly"/> correspondente à data armazenada.
    /// Retorna <see cref="DateOnly.MinValue"/> se <paramref name="value"/> for <c>null</c>
    /// ou se o valor interno estiver vazio (ex: objeto criado pelo construtor padrão).
    /// </returns>
    public static DateOnly ToDateOnly(DataXsd? value) => value == null || string.IsNullOrEmpty(value.Value) ? default
        : DateOnly.ParseExact(value.Value, DefaultDateFormat, CultureInfo.InvariantCulture);

    /// <inheritdoc cref="FromDateTime"/>
    public static explicit operator DataXsd(DateTime value) => FromDateTime(value);

    /// <inheritdoc cref="ToDateTime"/>
    public static implicit operator DateTime(DataXsd? value) => ToDateTime(value);

    /// <inheritdoc cref="ToDateOnly"/>
    public static implicit operator DateOnly(DataXsd? value) => ToDateOnly(value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="DataXsd"/>.
    /// </summary>
    /// <param name="value">Data como <see cref="DateTime"/>, possivelmente nula.</param>
    /// <returns>Nova instância de <see cref="DataXsd"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> for inválido.</exception>
    public static DataXsd? ParseIfPresent(DateTime? value) =>
        value == null ? null : FromDateTime(value.Value);

    /// <summary>
    /// Retorna <see langword="null"/> se <paramref name="value"/> for nulo;
    /// caso contrário, cria uma instância de <see cref="DataXsd"/>.
    /// </summary>
    /// <param name="value">Data como <see cref="DateOnly"/>, possivelmente nula.</param>
    /// <returns>Nova instância de <see cref="DataXsd"/> ou <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">Se <paramref name="value"/> estiver fora do intervalo válido.</exception>
    public static DataXsd? ParseIfPresent(DateOnly? value) =>
        value == null ? null : FromDateOnly(value.Value);
}