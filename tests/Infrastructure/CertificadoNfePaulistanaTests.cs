using Nfe.Paulistana.Options;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.Infrastructure;

/// <summary>
/// Testes unitários para <see cref="Certificado"/>:
/// sanitização de propriedades e comportamento de <see cref="Certificado.Build"/>.
/// </summary>
public class CertificadoNfePaulistanaTests
{
    private static X509Certificate2 CriarCertificado()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=Teste", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
    }

    // ============================================
    // FilePath — sanitização
    // ============================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FilePath_SetToEmptyOrWhitespace_BecomesNull(string value)
    {
        var config = new Certificado { FilePath = value };

        Assert.Null(config.FilePath);
    }

    [Fact]
    public void FilePath_SetToValidPath_RetainsValue()
    {
        const string path = @"C:\certificado.pfx";
        var config = new Certificado { FilePath = path };

        Assert.Equal(path, config.FilePath);
    }

    [Fact]
    public void FilePath_SetToNull_BecomesNull()
    {
        var config = new Certificado { FilePath = null };

        Assert.Null(config.FilePath);
    }

    // ============================================
    // Password — sanitização
    // ============================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Password_SetToEmptyOrWhitespace_BecomesNull(string value)
    {
        var config = new Certificado { Password = value };

        Assert.Null(config.Password);
    }

    [Fact]
    public void Password_SetToValidValue_RetainsValue()
    {
        var config = new Certificado { Password = "senha123" };

        Assert.Equal("senha123", config.Password);
    }

    // ============================================
    // PointerHandle — sanitização
    // ============================================

    [Fact]
    public void PointerHandle_SetToZero_BecomesNull()
    {
        var config = new Certificado { PointerHandle = IntPtr.Zero };

        Assert.Null(config.PointerHandle);
    }

    [Fact]
    public void PointerHandle_SetToNonZero_RetainsValue()
    {
        nint handle = new(42);
        var config = new Certificado { PointerHandle = handle };

        Assert.Equal(handle, config.PointerHandle);
    }

    // ============================================
    // RawData — sanitização
    // ============================================

    [Fact]
    public void RawData_SetToEmptyCollection_BecomesNull()
    {
        var config = new Certificado { RawData = new ReadOnlyCollection<byte>([]) };

        Assert.Null(config.RawData);
    }

    [Fact]
    public void RawData_SetToNonEmptyCollection_RetainsValue()
    {
        var data = new ReadOnlyCollection<byte>([0x01, 0x02]);
        var config = new Certificado { RawData = data };

        Assert.Equal(data, config.RawData);
    }

    // ============================================
    // BuildCertificate — comportamento
    // ============================================

    [Fact]
    public void BuildCertificate_SemFonteConfigurada_ThrowsInvalidOperationException()
    {
        var config = new Certificado();

        Assert.Throws<InvalidOperationException>(() => config.Build());
    }

    [Fact]
    public void BuildCertificate_ComCertificate_RetornaCertificadoComMesmoThumbprint()
    {
        using X509Certificate2 original = CriarCertificado();
        var config = new Certificado { Certificate = original };

        using X509Certificate2 resultado = config.Build();

        Assert.Equal(original.Thumbprint, resultado.Thumbprint);
    }

    [Fact]
    public void BuildCertificate_ComCertificate_RetornaNovaInstancia()
    {
        using X509Certificate2 original = CriarCertificado();
        var config = new Certificado { Certificate = original };

        using X509Certificate2 resultado = config.Build();

        Assert.NotSame(original, resultado);
    }

    [Fact]
    public void BuildCertificate_ComRawData_RetornaCertificadoComMesmoThumbprint()
    {
        using X509Certificate2 original = CriarCertificado();
        byte[] pfxBytes = original.Export(X509ContentType.Pkcs12);
        var config = new Certificado { RawData = new ReadOnlyCollection<byte>(pfxBytes) };

        using X509Certificate2 resultado = config.Build();

        Assert.Equal(original.Thumbprint, resultado.Thumbprint);
    }

    [Fact]
    public void BuildCertificate_Chama_Multiplas_Vezes_RetornaInstanciasDiferentes()
    {
        using X509Certificate2 cert = CriarCertificado();
        var config = new Certificado { Certificate = cert };

        using X509Certificate2 r1 = config.Build();
        using X509Certificate2 r2 = config.Build();

        Assert.NotSame(r1, r2);
    }
}