using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Models.Domain;

public class GrupoValorInclusoTests
{
    [Fact]
    public void DefaultConstructor_DocumentosIsNull()
    {
        var g = new GrupoValorIncluso();

        Assert.Null(g.Documentos);
    }

    [Fact]
    public void Constructor_WithOneDocument_SetsArray()
    {
        // Arrange
        var doc = new Documento(
            new DataXsd(new DateTime(2024, 1, 1)),
            new DataXsd(new DateTime(2024, 1, 1)),
            TipoValorIncluso.RepasseIntermediacaoImoveis,
            new Valor(100m),
            new DocumentoFiscalNacional(),
            null,
            null,
            null,
            null
        );

        var list = new[] { doc };

        // Act
        var g = new GrupoValorIncluso(list);

        // Assert
        Assert.NotNull(g.Documentos);
        Assert.Single(g.Documentos);
        Assert.Equal(doc, g.Documentos![0]);
    }

    [Fact]
    public void Constructor_EmptyEnumerable_ThrowsArgumentException()
    {
        var empty = Array.Empty<Documento>();

        _ = Assert.Throws<ArgumentException>(() => new GrupoValorIncluso(empty));
    }

    [Fact]
    public void Constructor_NullEnumerable_ThrowsArgumentNullException()
    {
        IEnumerable<Documento>? docs = null;

        _ = Assert.Throws<ArgumentNullException>(() => new GrupoValorIncluso(docs!));
    }

    [Fact]
    public void Constructor_TooManyItems_ThrowsArgumentException()
    {
        var items = Enumerable.Range(0, 101)
            .Select(i => new Documento(
                new DataXsd(new DateTime(2024, 1, 1)),
                new DataXsd(new DateTime(2024, 1, 1)),
                TipoValorIncluso.RepasseIntermediacaoImoveis,
                new Valor(100m),
                new DocumentoFiscalNacional(),
                null,
                null,
                null,
                null
            ))
            .ToList();

        _ = Assert.Throws<ArgumentException>(() => new GrupoValorIncluso(items));
    }

    [Fact]
    public void Constructor_MaxAllowedItems_Succeeds()
    {
        var items = Enumerable.Range(0, 100)
            .Select(i => new Documento(
                new DataXsd(new DateTime(2024, 1, 1)),
                new DataXsd(new DateTime(2024, 1, 1)),
                TipoValorIncluso.RepasseIntermediacaoImoveis,
                new Valor(100m),
                new DocumentoFiscalNacional(),
                null,
                null,
                null,
                null
            ))
            .Take(100)
            .ToList();

        var g = new GrupoValorIncluso(items);

        Assert.NotNull(g.Documentos);
        Assert.Equal(items.Count, g.Documentos!.Length);
    }
}