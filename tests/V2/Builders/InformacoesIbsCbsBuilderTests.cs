using Nfe.Paulistana.V2.Builders;
using Nfe.Paulistana.V2.Models.DataTypes;
using Nfe.Paulistana.V2.Models.Domain;
using Nfe.Paulistana.V2.Models.Enums;

namespace Nfe.Paulistana.Tests.V2.Builders;

/// <summary>
/// Testes unitários para <see cref="InformacoesIbsCbsBuilder"/>.
/// </summary>
public sealed class InformacoesIbsCbsBuilderTests
{
    [Fact]
    public void New_CriaNovaInstanciaDoBuilder()
    {
        // Arrange & Act
        var builder = InformacoesIbsCbsBuilder.New();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void SetFinalidadeEmissao_RetornaProprioBuilder()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act
        var result = builder.SetFinalidadeEmissao(FinalidadeEmissaoNfe.NfseRegular);

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void SetUsoOuConsumoPessoal_ArgumentoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => builder.SetUsoOuConsumoPessoal(null!));
    }

    [Fact]
    public void SetUsoOuConsumoPessoal_RetornaProprioBuilder()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();
        var usoOuConsumo = new NaoSim();

        // Act
        var result = builder.SetUsoOuConsumoPessoal(usoOuConsumo);

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void SetDestinatarioServicos_RetornaProprioBuilder()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act
        var result = builder.SetDestinatarioServicos(DestinatarioServicos.ProprioTomador);

        // Assert
        Assert.Same(builder, result);
    }

    [Fact]
    public void SetClassificacaoTributaria_ArgumentoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => builder.SetClassificacaoTributaria(null!));
    }

    [Fact]
    public void SetCodigoOperacaoFornecimento_ArgumentoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => builder.SetCodigoOperacaoFornecimento(null!));
    }

    [Fact]
    public void SetGrupoNfeReferenciada_ArgumentoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => builder.SetGrupoNfeReferenciada(null!));
    }

    [Fact]
    public void SetDestinatario_ArgumentoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => builder.SetDestinatario(null!));
    }

    [Fact]
    public void SetImovelObra_ArgumentoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => builder.SetImovelObra(null!));
    }

    [Fact]
    public void Build_SemUsoOuConsumoPessoal_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("UsoOuConsumoPessoal", exception.Message);
    }

    [Fact]
    public void Build_SemCodigoOperacaoFornecimento_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();
        var usoOuConsumo = new NaoSim();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.SetUsoOuConsumoPessoal(usoOuConsumo).Build());
        Assert.Contains("CodigoOperacaoFornecimento", exception.Message);
    }

    [Fact]
    public void Build_SemValores_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();
        var usoOuConsumo = new NaoSim();
        var codigo = new CodigoOperacao("010101"); // 6 dígitos

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder
                .SetUsoOuConsumoPessoal(usoOuConsumo)
                .SetCodigoOperacaoFornecimento(codigo)
                .Build());
        Assert.Contains("Valores", exception.Message);
    }

    [Fact]
    public void AddDocumentosReembolsoRepasseOuRessarcimento_SemClassificacaoTributaria_ThrowsInvalidOperationException()
    {
        // Arrange
        var builder = InformacoesIbsCbsBuilder.New();
        var documento = new Documento();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.AddDocumentosReembolsoRepasseOuRessarcimento(documento));
        Assert.Contains("classificação tributária", exception.Message);
    }
}
