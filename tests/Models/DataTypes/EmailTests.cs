using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class EmailTests
{
    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new Email().ToString());
    }

    // ============================================
    // Email(string) — formato válido
    // ============================================

    [Theory]
    [InlineData("usuario@empresa.com.br")]
    [InlineData("teste.nfe@prefeitura.sp.gov.br")]
    [InlineData("a@b.co")]
    public void Constructor_WithValidEmail_StoresCorrectly(string email)
    {
        Assert.Equal(email, new Email(email).ToString());
    }

    [Fact]
    public void Constructor_WithMaxLengthEmail_StoresCorrectly()
    {
        // 75 chars: local(63) + @ + domain(11) = 75
        string local = new('a', 63);
        string email = $"{local}@bcde.com.br";
        Assert.Equal(75, email.Length);
        Assert.Equal(email, new Email(email).ToString());
    }

    // ============================================
    // Email(string) — formatos inválidos
    // ============================================

    [Theory]
    [InlineData("nao-e-email")]
    [InlineData("@semlocal.com")]
    [InlineData("semdominio@")]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidEmail_ThrowsArgumentException(string invalid)
    {
        Assert.Throws<ArgumentException>(() => new Email(invalid));
    }

    [Fact]
    public void Constructor_ExceedingMaxLength_ThrowsArgumentException()
    {
        // 76 chars: local(64) + @ + domain(11) = 76
        string local = new('a', 64);
        string email = $"{local}@bcde.com.br";
        Assert.Equal(76, email.Length);
        Assert.Throws<ArgumentException>(() => new Email(email));
    }

    // ============================================
    // Factory / operador de cast
    // ============================================

    [Fact]
    public void FromString_WithValidEmail_StoresCorrectly()
    {
        Assert.Equal("nfe@sp.gov.br", Email.FromString("nfe@sp.gov.br").ToString());
    }

    [Fact]
    public void CastOperator_WithValidEmail_StoresCorrectly()
    {
        Assert.Equal("nfe@sp.gov.br", ((Email)"nfe@sp.gov.br").ToString());
    }

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void IsSealed()
    {
        Assert.True(typeof(Email).IsSealed);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new Email("a@b.com"), new Email("a@b.com"));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new Email("a@b.com"), new Email("c@d.com"));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsEqualHash()
    {
        Assert.Equal(new Email("a@b.com").GetHashCode(), new Email("a@b.com").GetHashCode());
    }

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(Email.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(Email.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(Email.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("nfe@sp.gov.br", Email.ParseIfPresent("nfe@sp.gov.br")!.ToString());

    [Fact]
    public void ParseIfPresent_InvalidEmailFormatInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Email.ParseIfPresent("nao-e-email"));

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException()
    {
        string local = new('a', 64);
        string email = $"{local}@bcde.com.br"; // 76 chars
        Assert.Throws<ArgumentException>(() => Email.ParseIfPresent(email));
    }
}
