using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class CodigoServicoTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new CodigoServico().ToString());
    }

    [Fact]
    public void Constructor_WithValidCode_StoresCorrectly()
    {
        Assert.Equal("7617", new CodigoServico(7617).ToString());
    }

    [Fact]
    public void Constructor_WithMinValidValue_Accepted()
    {
        Assert.Equal("1000", new CodigoServico(1_000).ToString());
    }

    [Fact]
    public void Constructor_WithMaxValidValue_Accepted()
    {
        Assert.Equal("99999", new CodigoServico(99_999).ToString());
    }

    [Fact]
    public void Constructor_BelowMin_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CodigoServico(999));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AboveMax_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CodigoServico(100_000));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WithString_StoresCorrectly()
    {
        Assert.Equal("7617", new CodigoServico("7617").ToString());
    }

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => new CodigoServico("SERV"));
    }

    [Fact]
    public void FromString_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new CodigoServico("7617").ToString(), CodigoServico.FromString("7617").ToString());
    }

    [Fact]
    public void FromInt32_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new CodigoServico(7617).ToString(), CodigoServico.FromInt32(7617).ToString());
    }

    [Fact]
    public void ExplicitCast_FromInt_ProducesSameResultAsFromInt32()
    {
        Assert.Equal(CodigoServico.FromInt32(7617).ToString(), ((CodigoServico)7617).ToString());
    }

    [Fact]
    public void CodigoServico_IsSealed() => Assert.True(typeof(CodigoServico).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new CodigoServico(7617), new CodigoServico(7617));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new CodigoServico(7617), new CodigoServico(9999));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new CodigoServico(7617).GetHashCode(), new CodigoServico(7617).GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new CodigoServico(7617).GetHashCode(), new CodigoServico(9999).GetHashCode());

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new CodigoServico(7617).Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        var servico = new CodigoServico(1_000);
        var numero = new Numero(1_000);

        Assert.False(servico.Equals(numero));
    }

    [Fact]
    public void ExplicitCast_FromString_ProducesSameResultAsFromString() =>
        Assert.Equal(CodigoServico.FromString("7617").ToString(), ((CodigoServico)"7617").ToString());

    [Fact]
    public void ExplicitCast_FromString_WithInvalidInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (CodigoServico)"SERV"; });

    [Fact]
    public void ExplicitCast_FromInt_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => { var _ = (CodigoServico)999; });
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(CodigoServico.ParseIfPresent((string?)null));

    [Fact]
    public void ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(CodigoServico.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_WithValidString_ReturnsCodigoServico() =>
        Assert.Equal(new CodigoServico(7617), CodigoServico.ParseIfPresent("7617"));

    [Fact]
    public void ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoServico.ParseIfPresent("SERV"));

    // ============================================
    // ParseIfPresent(int?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullInt_ReturnsNull() =>
        Assert.Null(CodigoServico.ParseIfPresent((int?)null));

    [Fact]
    public void ParseIfPresent_WithValidInt_ReturnsCodigoServico() =>
        Assert.Equal(new CodigoServico(7617), CodigoServico.ParseIfPresent((int?)7617));

    [Fact]
    public void ParseIfPresent_WithInvalidInt_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoServico.ParseIfPresent((int?)999));
}
