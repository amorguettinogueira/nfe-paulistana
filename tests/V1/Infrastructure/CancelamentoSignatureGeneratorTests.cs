using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.V1.Infrastructure;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Infrastructure;

/// <summary>
/// Testes unitários para <see cref="CancelamentoSignatureGenerator"/> (V1).
/// </summary>
public sealed class CancelamentoSignatureGeneratorTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
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
    // Contrato de assinatura
    // ============================================

    [Fact]
    public void Sign_DetalheValido_AssinaturaNaoEhNula()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
    }

    [Fact]
    public void Sign_DetalheValido_AssinaturaContemBytes()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.True(detalhe.AssinaturaCancelamento!.Length > 0);
    }

    [Fact]
    public void Sign_ComInscricacaoENumeroDistintos_GeraAssinaturaValida()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        var detalhe = new DetalheCancelamentoNFe(new ChaveNfe(
            new InscricaoMunicipal(12345678),
            new Numero(987654)));
        X509Certificate2 certificate = fixture.Certificate;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.True(detalhe.AssinaturaCancelamento!.Length > 0);
    }

    [Fact]
    public void Sign_ChamadoDuasVezes_AssinaturaEhSobreescrita()
    {
        // Arrange
        var generator = new CancelamentoSignatureGenerator();
        DetalheCancelamentoNFe detalhe = CriarDetalhePadrao();
        X509Certificate2 certificate = fixture.Certificate;

        generator.Sign(detalhe, certificate);
        byte[] primeiraAssinatura = detalhe.AssinaturaCancelamento!;

        // Act
        generator.Sign(detalhe, certificate);

        // Assert
        Assert.NotNull(detalhe.AssinaturaCancelamento);
        Assert.Equal(primeiraAssinatura.Length, detalhe.AssinaturaCancelamento!.Length);
    }
}
