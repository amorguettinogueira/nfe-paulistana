using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class GrupoNfeReferenciadaTests
{
    private static readonly string ChaveValida = new string('A', 40) + "1234567890";

    [Fact]
    public void DefaultConstructor_NfeReferenciadasIsNull()
    {
        var g = new GrupoNfeReferenciada();

        Assert.Null(g.NfeReferenciadas);
    }

    [Fact]
    public void Constructor_WithOneReference_SetsArray()
    {
        // Arrange
        var chave = new ChaveNotaNacional(ChaveValida);
        var list = new[] { chave };

        // Act
        var g = new GrupoNfeReferenciada(list);

        // Assert
        Assert.NotNull(g.NfeReferenciadas);
        Assert.Single(g.NfeReferenciadas);
        Assert.Equal(chave, g.NfeReferenciadas![0]);
    }

    [Fact]
    public void Constructor_WithMultipleReferences_SetsArrayInSameOrder()
    {
        // Arrange
        var a = new ChaveNotaNacional(ChaveValida);
        var b = new ChaveNotaNacional(new string('B', 40) + "1234567890");
        var c = new ChaveNotaNacional(new string('C', 40) + "1234567890");
        var list = new List<ChaveNotaNacional> { a, b, c };

        // Act
        var g = new GrupoNfeReferenciada(list);

        // Assert
        Assert.Equal(3, g.NfeReferenciadas?.Length);
        Assert.Equal(a, g.NfeReferenciadas![0]);
        Assert.Equal(b, g.NfeReferenciadas[1]);
        Assert.Equal(c, g.NfeReferenciadas[2]);
    }

    [Fact]
    public void Constructor_NullEnumerable_ThrowsArgumentNullException()
    {
        IEnumerable<ChaveNotaNacional>? list = null;

        _ = Assert.Throws<ArgumentNullException>(() => new GrupoNfeReferenciada(list!));
    }

    [Fact]
    public void Constructor_EmptyEnumerable_ThrowsArgumentException()
    {
        var empty = Array.Empty<ChaveNotaNacional>();

        _ = Assert.Throws<ArgumentException>(() => new GrupoNfeReferenciada(empty));
    }

    [Fact]
    public void Constructor_TooManyItems_ThrowsArgumentException()
    {
        // Arrange: create 100 items (limit is 99)
        var items = Enumerable.Range(0, 100)
            .Select(i => new ChaveNotaNacional(new string((char)('A' + (i % 26)), 40) + "1234567890"))
            .ToList();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(() => new GrupoNfeReferenciada(items));
    }

    [Fact]
    public void Constructor_NinetyNineItems_Succeeds()
    {
        // Arrange: create 99 valid items
        var items = Enumerable.Range(0, 99)
            .Select(i => new ChaveNotaNacional(new string((char)('A' + (i % 26)), 40) + "1234567890"))
            .ToList();

        // Act
        var g = new GrupoNfeReferenciada(items);

        // Assert
        Assert.NotNull(g.NfeReferenciadas);
        Assert.Equal(99, g.NfeReferenciadas!.Length);
    }
}
