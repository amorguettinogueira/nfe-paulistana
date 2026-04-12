using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Enums;
using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Builders;
using Nfe.Paulistana.V1.Infrastructure;
using Nfe.Paulistana.V1.Models.DataTypes;
using Nfe.Paulistana.V1.Models.Domain;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.V1.Infrastructure;

/// <summary>
/// Testes unitários para <see cref="RpsSignatureGenerator"/>:
/// guard clauses e verificação de que <see cref="Rps.Assinatura"/> é populada corretamente.
/// </summary>
public class RpsSignatureGeneratorTests
{
    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Teste", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
    }

    private static readonly Tomador TomadorPadrao =
        TomadorBuilder.NewCpf(new Cpf(new ValidCpfNumber().Min())).Build();

    private static Rps CriarRps() =>
        RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(4105),
                new Discriminacao("Desenvolvimento de software."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), (Valor)1000m)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(TomadorPadrao)
            .Build();

    // ============================================
    // Guard clauses
    // ============================================

    [Fact]
    public void Sign_RpsNulo_ThrowsArgumentNullException()
    {
        var generator = new RpsSignatureGenerator();
        Rps? rps = null;
        using X509Certificate2 certificate = CriarCertificado();

        _ = Assert.Throws<ArgumentNullException>(() => generator.Sign(rps!, certificate));
    }

    [Fact]
    public void Sign_CertificadoNulo_ThrowsArgumentNullException()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = CriarRps();
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
        Rps rps = CriarRps();
        using X509Certificate2 certificate = CriarCertificado();

        generator.Sign(rps, certificate);

        Assert.NotNull(rps.Assinatura);
    }

    [Fact]
    public void Sign_RpsECertificadoValidos_AssinaturaContemBytes()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = CriarRps();
        using X509Certificate2 certificate = CriarCertificado();

        generator.Sign(rps, certificate);

        Assert.True(rps.Assinatura!.Length > 0);
    }

    [Fact]
    public void Sign_ChamadoDuasVezes_AssinaturaEhSobreescrita()
    {
        var generator = new RpsSignatureGenerator();
        Rps rps = CriarRps();
        using X509Certificate2 certificate = CriarCertificado();

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
        using X509Certificate2 certificate = CriarCertificado();

        var intermediario = IntermediarioBuilder
            .New(new Cpf(new ValidCpfNumber().Min()), false)
            .Build();

        Rps rps = RpsBuilder.New(
                new InscricaoMunicipal(39616924),
                TipoRps.Rps,
                new Numero(4106),
                new Discriminacao("Desenvolvimento de software com intermediário."),
                new SerieRps("BB"))
            .SetNFe(new DataXsd(new DateTime(2024, 1, 20)), (TributacaoNfe)'T', StatusNfe.Normal)
            .SetServico(new CodigoServico(7617), (Valor)1000m)
            .SetIss((Aliquota)0.05m, false)
            .SetTomador(TomadorPadrao)
            .SetIntermediario(intermediario)
            .Build();

        // Act
        generator.Sign(rps, certificate);

        // Assert
        Assert.NotNull(rps.Assinatura);
        Assert.True(rps.Assinatura!.Length > 0);
    }
}