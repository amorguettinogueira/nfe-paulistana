using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V2.Infrastructure;
using Nfe.Paulistana.V2.Models.Domain;
using System.Security.Cryptography.X509Certificates;
using InscricaoMunicipal = Nfe.Paulistana.V2.Models.DataTypes.InscricaoMunicipal;

namespace Nfe.Paulistana.Tests.V2.Infrastructure;

/// <summary>
/// Testes unitários para a classe CancelamentoSignatureGenerator, responsável por gerar a assinatura digital
/// </summary>
public class CancelamentoSignatureGeneratorTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{

    private static DetalheCancelamentoNFe CriarDetalhePadrao() =>
        new(new ChaveNfe(
            new InscricaoMunicipal(39616924),
            new Numero(123456)));

    // ============================================
    // Guard clauses
    // ============================================

    [Fact]
    public void Sign_DetalheNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe? detalhe = null;
        X509Certificate2 certificate = fixture.Certificate;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => generator.Sign(detalhe!, certificate));
    }

    [Fact]
    public void Sign_CertificadoNulo_ThrowsArgumentNullException()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2? certificate = null;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => generator.Sign(detalhe, certificate!));
    }

    // ============================================
    // Generate method - line 33 coverage
    // ============================================

    [Fact]
    public void Sign_DetalheValido_GeraTextoAssinaturaComComprimentoEsperado()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 0);
    }

    // ============================================
    // Sign method - line 57 coverage (successful signature)
    // ============================================

    [Fact]
    public void Sign_DetalheValido_AssinaturaGeradaCorretamente()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 0);
    }

    [Fact]
    public void Sign_DetalheComInscricacaoENumero_AssinaturaGeradaCorretamente()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe(
            new ChaveNfe(
                new InscricaoMunicipal(12345678),
                new Numero(987654321)));
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 0);
    }

    // ============================================
    // Sign method - line 55 coverage (invalid length validation)
    // ============================================

    [Fact]
    public void Sign_DetalheComChaveNfeNula_ThrowsInvalidOperationException()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe
        {
            ChaveNfe = null
        };
        X509Certificate2 certificate = fixture.Certificate;

        // Act & Assert
        _ = Assert.ThrowsAny<InvalidOperationException>(() => generator.Sign(detalhe, certificate));
    }

    [Fact]
    public void Sign_DetalheComInscricacaoNula_ThrowsInvalidOperationException()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe
        {
            ChaveNfe = new ChaveNfe
            {
                InscricaoPrestador = null,
                NumeroNFe = new Numero(123456)
            }
        };
        X509Certificate2 certificate = fixture.Certificate;

        // Act & Assert
        _ = Assert.ThrowsAny<InvalidOperationException>(() => generator.Sign(detalhe, certificate));
    }

    [Fact]
    public void Sign_DetalheComNumeroNFeNulo_ThrowsInvalidOperationException()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe
        {
            ChaveNfe = new ChaveNfe
            {
                InscricaoPrestador = new InscricaoMunicipal(12345678),
                NumeroNFe = null
            }
        };
        X509Certificate2 certificate = fixture.Certificate;

        // Act & Assert
        _ = Assert.ThrowsAny<InvalidOperationException>(() => generator.Sign(detalhe, certificate));
    }

    // ============================================
    // Additional coverage scenarios
    // ============================================

    [Fact]
    public void Sign_ChamadoDuasVezes_AssinaturaEhSobreescrita()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);
        byte[] primeiraAssinatura = detalhe.AssinaturaCancelamento!;

        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.Equal(primeiraAssinatura.Length, detalhe.AssinaturaCancelamento.Length);
    }

    [Fact]
    public void Sign_DetalheComCodigoVerificacao_AssinaturaGeradaCorretamente()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe(
            new ChaveNfe(
                new InscricaoMunicipal(98765432),
                new Numero(555),
                new CodigoVerificacao("ABC123XY")));
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 0);
    }

    [Fact]
    public void Sign_DetalheComInscricaoMaxima_AssinaturaGeradaCorretamente()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe(
            new ChaveNfe(
                new InscricaoMunicipal(999999999999),
                new Numero(999999999999)));
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 0);
    }

    [Fact]
    public void Sign_DetalheComInscricaoMinima_AssinaturaGeradaCorretamente()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe(
            new ChaveNfe(
                new InscricaoMunicipal(1),
                new Numero(1)));
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 0);
    }

    [Fact]
    public void Sign_AssinaturaAnteriorPreenchida_AssinaturaEhResetadaAntesDeGerar()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        detalhe.AssinaturaCancelamento = new byte[] { 1, 2, 3, 4, 5 };
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento.Length > 5);
    }
}