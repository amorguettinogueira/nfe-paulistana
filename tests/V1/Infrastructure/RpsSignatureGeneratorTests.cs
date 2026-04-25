using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Tests.Fixtures;
using Nfe.Paulistana.Tests.V1.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Infrastructure;
using Nfe.Paulistana.V1.Models.Domain;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Infrastructure;

/// <summary>
/// Testes unitários para <see cref="RpsSignatureGenerator"/>:
/// guard clauses e verificação de que <see cref="Rps.Assinatura"/> é populada corretamente.
/// </summary>
public class RpsSignatureGeneratorTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    // ============================================
    // Guard clauses
    // ============================================

    [Fact]
    public void Sign_RpsNulo_ThrowsArgumentNullException()
    {
        var generator = new RpsSignatureGenerator();
        Rps? rps = null;
        X509Certificate2 certificate = fixture.Certificate;

        _ = Assert.Throws<ArgumentNullException>(() => generator.Sign(rps!, certificate));
    }

    [Fact]
    public void Sign_CertificadoNulo_ThrowsArgumentNullException()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = RpsTestFactory.Padrao();
        X509Certificate2? certificate = null;

        _ = Assert.Throws<ArgumentNullException>(() => generator.Sign(rps, certificate!));
    }

    // ============================================
    // Contrato de assinatura
    // ============================================

    [Fact]
    public void Sign_RpsECertificadoValidos_AssinaturaNaoEhNula()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = RpsTestFactory.Padrao();
        X509Certificate2 certificate = fixture.Certificate;

        generator.Sign(rps, certificate);

        Assert.NotNull(rps.Assinatura);
    }

    [Fact]
    public void Sign_RpsECertificadoValidos_AssinaturaContemBytes()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = RpsTestFactory.Padrao();
        X509Certificate2 certificate = fixture.Certificate;

        generator.Sign(rps, certificate);

        Assert.True(rps.Assinatura!.Length > 0);
    }

    [Fact]
    public void Sign_ChamadoDuasVezes_AssinaturaEhSobreescrita()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = RpsTestFactory.Padrao();
        X509Certificate2 certificate = fixture.Certificate;

        generator.Sign(rps, certificate);
        byte[] primeiraAssinatura = rps.Assinatura!;

        generator.Sign(rps, certificate);

        Assert.NotNull(rps.Assinatura);
        Assert.Equal(primeiraAssinatura.Length, rps.Assinatura.Length);
    }

    // ============================================
    // Caminho com intermediário (102 chars)
    // ============================================

    [Fact]
    public void Sign_RpsComIntermediario_AssinaturaContemBytes()
    {
        // Arrange
        var generator = new RpsSignatureGenerator();
        X509Certificate2 certificate = fixture.Certificate;

        var intermediario = IntermediarioBuilder
            .New((Cpf)Tests.Helpers.TestConstants.ValidCpf, false)
            .Build();

        Rps rps = RpsTestFactory.Padrao(intermediario: intermediario);

        // Act
        generator.Sign(rps, certificate);

        // Assert
        Assert.NotNull(rps.Assinatura);
        Assert.True(rps.Assinatura!.Length > 0);
    }
}