using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="ChaveNotaNacional"/>:
/// construtor padrão, validação de formato, happy path e desserialização.
/// </summary>
public class ChaveNotaNacionalTests
{
    private static readonly string ChaveValida = new string('A', 40) + "1234567890";

    // ============================================
    // Construtor padrão (desserialização)
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new ChaveNotaNacional().ToString());
    }

    // ============================================
    // Happy path
    // ============================================

    [Fact]
    public void Constructor_ChaveValida_ArmazenaValor()
    {
        var chave = new ChaveNotaNacional(ChaveValida);

        Assert.Equal(ChaveValida, chave.ToString());
    }

    [Fact]
    public void Constructor_ChaveComApenasDigitos_Aceita()
    {
        var valor = new string('0', 50);

        var chave = new ChaveNotaNacional(valor);

        Assert.Equal(valor, chave.ToString());
    }

    [Fact]
    public void Constructor_ChaveComApenasLetras_Aceita()
    {
        var valor = new string('Z', 50);

        var chave = new ChaveNotaNacional(valor);

        Assert.Equal(valor, chave.ToString());
    }

    // ============================================
    // Validação de formato — erros
    // ============================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ValorVazioOuBranco_ThrowsArgumentException(string valor)
    {
        _ = Assert.Throws<ArgumentException>(() => new ChaveNotaNacional(valor));
    }

    [Fact]
    public void Constructor_ValorNulo_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new ChaveNotaNacional(null!));
    }

    [Fact]
    public void Constructor_ChaveCom49Caracteres_ThrowsArgumentException()
    {
        var valor = new string('A', 49);

        _ = Assert.Throws<ArgumentException>(() => new ChaveNotaNacional(valor));
    }

    [Fact]
    public void Constructor_ChaveCom51Caracteres_ThrowsArgumentException()
    {
        var valor = new string('A', 51);

        _ = Assert.Throws<ArgumentException>(() => new ChaveNotaNacional(valor));
    }

    [Fact]
    public void Constructor_ChaveComLetrasMinusculas_ThrowsArgumentException()
    {
        var valor = new string('a', 50);

        var ex = Assert.Throws<ArgumentException>(() => new ChaveNotaNacional(valor));
        Assert.Contains("50 caracteres", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_ChaveComCaracteresEspeciais_ThrowsArgumentException()
    {
        var valor = new string('A', 49) + "-";

        _ = Assert.Throws<ArgumentException>(() => new ChaveNotaNacional(valor));
    }

    // ============================================
    // FromString e operador explícito
    // ============================================

    [Fact]
    public void FromString_ProducesMesmoResultadoQueConstructor()
    {
        var viaConstructor = new ChaveNotaNacional(ChaveValida);
        var viaFactory = ChaveNotaNacional.FromString(ChaveValida);

        Assert.Equal(viaConstructor.ToString(), viaFactory.ToString());
    }

    [Fact]
    public void OperadorExplicito_ProducesMesmoResultadoQueConstructor()
    {
        var viaConstructor = new ChaveNotaNacional(ChaveValida);
        var viaOperador = (ChaveNotaNacional)ChaveValida;

        Assert.Equal(viaConstructor.ToString(), viaOperador.ToString());
    }

    // ============================================
    // Igualdade por valor
    // ============================================

    [Fact]
    public void Equals_MesmoValor_Verdadeiro()
    {
        var a = new ChaveNotaNacional(ChaveValida);
        var b = new ChaveNotaNacional(ChaveValida);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_ValoresDiferentes_Falso()
    {
        var a = new ChaveNotaNacional(ChaveValida);
        var b = new ChaveNotaNacional(new string('B', 40) + "1234567890");

        Assert.NotEqual(a, b);
    }
}
