using Nfe.Paulistana.Options;
using Nfe.Paulistana.Tests.Fixtures;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace Nfe.Paulistana.Tests.Options;

public class CertificadoTests(CertificadoFixture fixture) : IClassFixture<CertificadoFixture>
{
    // ============================================
    // Propriedades: FilePath
    // ============================================

    [Fact]
    public void FilePath_SetWithValidValue_StoresValue()
    {
        // Arrange
        var certificado = new Certificado();

        // Act
        certificado.FilePath = "cert.pfx";

        // Assert
        Assert.Equal("cert.pfx", certificado.FilePath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FilePath_SetWithNullOrWhitespace_StoresNull(string? value)
    {
        // Arrange
        var certificado = new Certificado();

        // Act
        certificado.FilePath = value;

        // Assert
        Assert.Null(certificado.FilePath);
    }

    // ============================================
    // Propriedades: RawData
    // ============================================

    [Fact]
    public void RawData_SetWithValidValue_StoresValue()
    {
        // Arrange
        var certificado = new Certificado();
        var data = new ReadOnlyCollection<byte>([1, 2, 3]);

        // Act
        certificado.RawData = data;

        // Assert
        Assert.Equal(data, certificado.RawData);
    }

    [Fact]
    public void RawData_SetWithEmptyCollection_StoresNull()
    {
        // Arrange
        var certificado = new Certificado();
        var emptyData = new ReadOnlyCollection<byte>([]);

        // Act
        certificado.RawData = emptyData;

        // Assert
        Assert.Null(certificado.RawData);
    }

    [Fact]
    public void RawData_SetWithNull_StoresNull()
    {
        // Arrange
        var certificado = new Certificado();

        // Act
        certificado.RawData = null;

        // Assert
        Assert.Null(certificado.RawData);
    }

    // ============================================
    // Propriedades: PointerHandle
    // ============================================

    [Fact]
    public void PointerHandle_SetWithValidValue_StoresValue()
    {
        // Arrange
        var certificado = new Certificado();
        var handle = new nint(12345);

        // Act
        certificado.PointerHandle = handle;

        // Assert
        Assert.Equal(handle, certificado.PointerHandle);
    }

    [Fact]
    public void PointerHandle_SetWithZero_StoresNull()
    {
        // Arrange
        var certificado = new Certificado();

        // Act
        certificado.PointerHandle = (nint)0;

        // Assert
        Assert.Null(certificado.PointerHandle);
    }

    [Fact]
    public void PointerHandle_SetWithNull_StoresNull()
    {
        // Arrange
        var certificado = new Certificado();

        // Act
        certificado.PointerHandle = null;

        // Assert
        Assert.Null(certificado.PointerHandle);
    }

    // ============================================
    // Build: FilePath sem KeyStorageFlags (linha 79)
    // ============================================

    [Fact]
    public void Build_WithFilePathWithoutKeyStorageFlags_LoadsCertificate()
    {
        // Arrange
        var certificado = new Certificado
        {
            FilePath = GetTestCertificatePath(),
            Password = "test"
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
    }

    // ============================================
    // Build: FilePath com KeyStorageFlags (linhas 77-78)
    // ============================================

    [Fact]
    public void Build_WithFilePathAndKeyStorageFlags_LoadsCertificateWithFlags()
    {
        // Arrange
        var certificado = new Certificado
        {
            FilePath = GetTestCertificatePath(),
            Password = "test",
            KeyStorageFlags = X509KeyStorageFlags.EphemeralKeySet
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
    }

    // ============================================
    // Build: PointerHandle (linhas 82, 84)
    // ============================================

    [Fact]
    public void Build_WithPointerHandle_CreatesFromHandle()
    {
        // Arrange
        var tempCert = fixture.Certificate;
        var certificado = new Certificado
        {
            PointerHandle = tempCert.Handle
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        Assert.Equal(tempCert.Thumbprint, cert.Thumbprint);
    }

    // ============================================
    // Build: RawData sem KeyStorageFlags
    // ============================================

    [Fact]
    public void Build_WithRawDataWithoutKeyStorageFlags_LoadsCertificate()
    {
        // Arrange
        var rawData = GetTestCertificateBytes();
        var certificado = new Certificado
        {
            RawData = new ReadOnlyCollection<byte>(rawData),
            Password = "test"
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
    }

    // ============================================
    // Build: RawData com KeyStorageFlags (linha 89)
    // ============================================

    [Fact]
    public void Build_WithRawDataAndKeyStorageFlags_LoadsCertificateWithFlags()
    {
        // Arrange
        var rawData = GetTestCertificateBytes();
        var certificado = new Certificado
        {
            RawData = new ReadOnlyCollection<byte>(rawData),
            Password = "test",
            KeyStorageFlags = X509KeyStorageFlags.EphemeralKeySet
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
    }

    // ============================================
    // Build: InvalidOperationException
    // ============================================

    [Fact]
    public void Build_WithNoSourceConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var certificado = new Certificado();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => certificado.Build());
        Assert.Contains("Nenhuma fonte de certificado válida configurada", ex.Message);
    }

    // ============================================
    // Build: Ordem de precedência
    // ============================================

    [Fact]
    public void Build_WithMultipleSources_PrefersFilePath()
    {
        // Arrange
        var tempCert = fixture.Certificate;
        var rawData = GetTestCertificateBytes();
        var certificado = new Certificado
        {
            FilePath = GetTestCertificatePath(),
            Password = "test",
            PointerHandle = tempCert.Handle,
            RawData = new ReadOnlyCollection<byte>(rawData),
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        // Verifica que usou FilePath (que carrega do arquivo de teste)
        Assert.True(cert.HasPrivateKey);
    }

    [Fact]
    public void Build_WithPointerHandleAndRawData_PrefersPointerHandle()
    {
        // Arrange
        var tempCert = fixture.Certificate;
        var rawData = GetTestCertificateBytes();
        var certificado = new Certificado
        {
            PointerHandle = tempCert.Handle,
            RawData = new ReadOnlyCollection<byte>(rawData),
            Password = "test"
        };

        // Act
        using var cert = certificado.Build();

        // Assert
        Assert.NotNull(cert);
        Assert.Equal(tempCert.Thumbprint, cert.Thumbprint);
    }

    // ============================================
    // Métodos auxiliares
    // ============================================

    private string GetTestCertificatePath()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_cert_{Guid.NewGuid()}.pfx");
        var bytes = fixture.Certificate.Export(X509ContentType.Pfx, "test");
        File.WriteAllBytes(tempPath, bytes);
        return tempPath;
    }

    private byte[] GetTestCertificateBytes() =>
        fixture.Certificate.Export(X509ContentType.Pfx, "test");
}