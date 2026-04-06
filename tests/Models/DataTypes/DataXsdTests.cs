using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class DataXsdTests
{
    // ============================================
    // Data() — Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ValueIsNull()
    {
        var date = new DataXsd();

        Assert.Null(date.ToString());
    }

    // ============================================
    // Data(DateTime) — Construtor com data
    // ============================================

    [Fact]
    public void Constructor_WithDateTime_StoresIsoFormattedDate()
    {
        var date = new DataXsd(new DateTime(2024, 1, 20));

        Assert.Equal("2024-01-20", date.ToString());
    }

    [Fact]
    public void Constructor_WithDateTimeHavingTimeComponent_DiscardTime()
    {
        var dateWithTime = new DateTime(2024, 6, 15, 23, 59, 59);
        var date = new DataXsd(dateWithTime);

        Assert.Equal("2024-06-15", date.ToString());
    }

    [Fact]
    public void Constructor_WithFirstDayOfMonth_FormatsWithLeadingZero()
    {
        var date = new DataXsd(new DateTime(2024, 3, 1));

        Assert.Equal("2024-03-01", date.ToString());
    }

    [Fact]
    public void Constructor_WithLastDayOfYear_FormatsCorrectly()
    {
        var date = new DataXsd(new DateTime(2023, 12, 31));

        Assert.Equal("2023-12-31", date.ToString());
    }

    [Fact]
    public void Constructor_WithSingleDigitMonthAndDay_PadsWithZero()
    {
        var date = new DataXsd(new DateTime(2024, 1, 5));

        Assert.Equal("2024-01-05", date.ToString());
    }

    [Fact]
    public void Constructor_UsesInvariantCulture_NotLocaleSensitive()
    {
        // dd/MM/yyyy é o formato pt-BR; garantimos que yyyy-MM-dd é usado independente de locale
        var date = new DataXsd(new DateTime(2024, 11, 9));

        Assert.Equal("2024-11-09", date.ToString());
        Assert.DoesNotContain("/", date.ToString());
    }

    // ============================================
    // FromDateTime — Factory method
    // ============================================

    [Fact]
    public void FromDateTime_ProducesSameResultAsConstructor()
    {
        var input = new DateTime(2024, 5, 22);

        var viaConstructor = new DataXsd(input);
        var viaFactory = DataXsd.FromDateTime(input);

        Assert.Equal(viaConstructor.ToString(), viaFactory.ToString());
    }

    [Fact]
    public void FromDateTime_WithTimeComponent_DiscardTime()
    {
        var dateWithTime = new DateTime(2024, 8, 10, 14, 30, 0);
        var date = DataXsd.FromDateTime(dateWithTime);

        Assert.Equal("2024-08-10", date.ToString());
    }

    // ============================================
    // ToDateTime — Conversão para DateTime
    // ============================================

    [Fact]
    public void ToDateTime_WithValidData_ReturnsCorrectDateTime()
    {
        var Data = new DataXsd(new DateTime(2024, 3, 15));
        var result = DataXsd.ToDateTime(Data);

        Assert.Equal(new DateTime(2024, 3, 15), result);
    }

    [Fact]
    public void ToDateTime_WithNull_ReturnsDateTimeMinValue()
    {
        var result = DataXsd.ToDateTime(null);

        Assert.Equal(DateTime.MinValue, result);
    }

    [Fact]
    public void ToDateTime_WithDefaultConstructor_ReturnsDateTimeMinValue()
    {
        // Data() cria instância com Value=null (para desserialização XML)
        var emptyData = new DataXsd();
        var result = DataXsd.ToDateTime(emptyData);
        Assert.Equal(DateTime.MinValue, result);
    }

    [Fact]
    public void ToDateTime_TimeComponentIsAlwaysMidnight()
    {
        var Data = new DataXsd(new DateTime(2024, 7, 4, 18, 30, 59));
        var result = DataXsd.ToDateTime(Data);

        Assert.Equal(TimeSpan.Zero, result.TimeOfDay);
    }

    // ============================================
    // ToDateOnly — Conversão para DateOnly
    // ============================================

    [Fact]
    public void ToDateOnly_WithValidData_ReturnsCorrectDateOnly()
    {
        var Data = new DataXsd(new DateOnly(2024, 3, 15));
        var result = DataXsd.ToDateOnly(Data);

        Assert.Equal(new DateOnly(2024, 3, 15), result);
    }

    [Fact]
    public void ToDateOnly_WithNull_ReturnsDateTimeMinValue()
    {
        var result = DataXsd.ToDateOnly(null);

        Assert.Equal(DateOnly.MinValue, result);
    }

    [Fact]
    public void ToDateOnly_WithDefaultConstructor_ReturnsDateOnlyMinValue()
    {
        // Data() cria instância com Value=null (para desserialização XML)
        var emptyData = new DataXsd();
        var result = DataXsd.ToDateOnly(emptyData);
        Assert.Equal(DateOnly.MinValue, result);
    }

    // ============================================
    // Explicit operator — (Data)DateTime
    // ============================================

    [Fact]
    public void ExplicitCast_FromDateTime_ProducesSameResultAsFromDateTime()
    {
        var input = new DateTime(2024, 9, 30);

        var viaFactory = DataXsd.FromDateTime(input);
        var viaCast = (DataXsd)input;

        Assert.Equal(viaFactory.ToString(), viaCast.ToString());
    }

    [Fact]
    public void ExplicitCast_WithTimeComponent_DiscardTime()
    {
        var dateWithTime = new DateTime(2024, 2, 29, 12, 0, 0); // 2024 é bissexto

        var date = (DataXsd)dateWithTime;

        Assert.Equal("2024-02-29", date.ToString());
    }

    // ============================================
    // Implicit operator — DateTime = Data?
    // ============================================

    [Fact]
    public void ImplicitCast_ValidData_ReturnsCorrectDateTime()
    {
        DataXsd Data = new(new DateTime(2024, 10, 1));
        DateTime result = Data;

        Assert.Equal(new DateTime(2024, 10, 1), result);
    }

    [Fact]
    public void ImplicitCast_NullData_ReturnsDateTimeMinValue()
    {
        DataXsd? Data = null;
        DateTime result = Data;

        Assert.Equal(DateTime.MinValue, result);
    }

    [Fact]
    public void ImplicitCast_DefaultConstructedData_ReturnsDateTimeMinValue()
    {
        DataXsd Data = new();
        DateTime result = Data;

        Assert.Equal(DateTime.MinValue, result);
    }

    // ============================================
    // Round-trip — DateTime → Data → DateTime
    // ============================================

    [Fact]
    public void RoundTrip_DateOnly_PreservesDate()
    {
        var original = new DateTime(2024, 6, 21);
        DataXsd Data = new(original);
        DateTime restored = Data;

        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_DateWithTime_PreservesDateDiscardTime()
    {
        var original = new DateTime(2024, 11, 15, 22, 45, 30);
        DataXsd Data = new(original);
        DateTime restored = Data;

        Assert.Equal(original.Date, restored);
        Assert.NotEqual(original, restored);
    }

    [Theory]
    [InlineData(2024, 1, 1)]
    [InlineData(2024, 2, 29)]  // ano bissexto
    [InlineData(2023, 12, 31)]
    [InlineData(2000, 6, 15)]
    public void RoundTrip_VariousDates_AllPreserveDate(int year, int month, int day)
    {
        var original = new DateTime(year, month, day);
        DataXsd Data = new(original);
        DateTime restored = Data;

        Assert.Equal(original, restored);
    }
}